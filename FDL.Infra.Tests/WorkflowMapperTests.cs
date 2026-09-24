using FDL.Core.Domain;
using FDL.Infra.Persistance;
using FDL.WF.Domain;

namespace FDL.Infra.Tests
{
    public class WorkflowMapperTests
    {
        private static readonly DateTime T0 = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);
        private static readonly ReferenceDossier Dossier = ReferenceDossier.DepuisExistant(1500123, 1);

        // Ligne valide par défaut : une note en cours, assignée à l'utilisateur 42.
        // Chaque test ne modifie que ce qui l'intéresse.
        private static WorkflowRow RowValide() => new()
        {
            IdWorkflow = 10,
            NumeroDossier = 1500123,
            Sequence = 1,
            Type = (int)WorkflowType.Note,
            Action = (int)WorkflowAction.ALire,
            Message = "Message test",
            IdUserAssigne = 42,
            IdGroupeAssigne = null,
            IdUserExpediteur = 1,
            DateExpedition = T0,
            IdUserTerminaison = null,
            DateTerminaison = null,
            IdDocument = 0
        };
        // ------------------------------------------------------------


        // --- Aller-retour Domain -> Row -> Domain -------------------

        [Theory]
        [InlineData("NoteUtilisateurEnCours")]
        [InlineData("DocumentGroupeTermine")]
        [InlineData("TacheGroupeEnCours")]
        public void AllerRetour_DomainVersRowVersDomain_ConserveToutesLesDonnees(string cas)
        {
            Workflow attendu = WorkflowDeTest(cas);

            Workflow obtenu = WorkflowMapper.ToDomain(WorkflowMapper.ToRow(attendu));

            Assert.Equal(attendu.IdWorkflow, obtenu.IdWorkflow);
            Assert.Equal(attendu.ReferenceDossier, obtenu.ReferenceDossier);
            Assert.Equal(attendu.Expediteur, obtenu.Expediteur);
            Assert.Equal(attendu.Assignation, obtenu.Assignation);
            Assert.Equal(attendu.Message, obtenu.Message);
            Assert.Equal(attendu.Type, obtenu.Type);
            Assert.Equal(attendu.Action, obtenu.Action);
            Assert.Equal(attendu.IdDocument, obtenu.IdDocument);
            Assert.Equal(attendu.Terminaison, obtenu.Terminaison);
        }

        private static Workflow WorkflowDeTest(string cas) => cas switch
        {
            "NoteUtilisateurEnCours" => Workflow.Reconstituer(10, Dossier, new AuditInfo(1, T0),
                WorkflowAssignation.PourUtilisateur(42), "Note", WorkflowType.Note, WorkflowAction.ALire, 0, null),

            // Dossier en notation héritée (séquence 0)
            "DocumentGroupeTermine" => Workflow.Reconstituer(11, ReferenceDossier.DepuisExistant(1500123, 0),
                new AuditInfo(1, T0), WorkflowAssignation.PourGroupe(10), "Document", WorkflowType.Document,
                WorkflowAction.ASigner, 502, new AuditInfo(42, T0.AddHours(3))),

            "TacheGroupeEnCours" => Workflow.Reconstituer(12, Dossier, new AuditInfo(1, T0),
                WorkflowAssignation.PourGroupe(20), "Tâche", WorkflowType.Tache, WorkflowAction.AValider, 0, null),

            _ => throw new ArgumentException($"Cas inconnu : {cas}", nameof(cas))
        };
        // ------------------------------------------------------------


        // --- ToRow --------------------------------------------------

        [Fact]
        public void ToRow_AssignationGroupe_RemplitSeulementIdGroupe()
        {
            var wf = Workflow.Reconstituer(10, Dossier, new AuditInfo(1, T0), WorkflowAssignation.PourGroupe(10),
                "Message", WorkflowType.Note, WorkflowAction.ALire, 0, null);

            WorkflowRow row = WorkflowMapper.ToRow(wf);

            Assert.Null(row.IdUserAssigne);
            Assert.Equal(10, row.IdGroupeAssigne);
        }

        [Fact]
        public void ToRow_EnCours_TerminaisonNulle()
        {
            WorkflowRow row = WorkflowMapper.ToRow(WorkflowMapper.ToDomain(RowValide()));

            Assert.Null(row.IdUserTerminaison);
            Assert.Null(row.DateTerminaison);
        }
        // ------------------------------------------------------------


        // --- ToDomain : lignes valides ------------------------------

        [Fact]
        public void ToDomain_TerminaisonComplete_ReconstitueLaTerminaison()
        {
            WorkflowRow row = RowValide();
            row.IdUserTerminaison = 42;
            row.DateTerminaison = T0.AddHours(3);

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.True(wf.EstTermine);
            Assert.Equal(new AuditInfo(42, T0.AddHours(3)), wf.Terminaison);
        }

        [Fact]
        public void ToDomain_AssignationGroupe_ReconstitueUnGroupe()
        {
            WorkflowRow row = RowValide();
            row.IdUserAssigne = null;
            row.IdGroupeAssigne = 10;

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.Equal(WorkflowAssignation.PourGroupe(10), wf.Assignation);
        }

        // --- ToDomain : règles de lecture des lignes incomplètes ----

        [Fact]
        public void ToDomain_UtilisateurEtGroupeRenseignes_UtilisateurPrioritaire()
        {
            WorkflowRow row = RowValide();
            row.IdUserAssigne = 42;
            row.IdGroupeAssigne = 10;

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.Equal(WorkflowAssignation.PourUtilisateur(42), wf.Assignation);
        }

        [Fact]
        public void ToDomain_TerminaisonSansDate_UtiliseDateUpdate()
        {
            WorkflowRow row = RowValide();
            row.IdUserTerminaison = 42;
            row.DateTerminaison = null;
            row.DateUpdate = T0.AddDays(2);

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.Equal(new AuditInfo(42, T0.AddDays(2)), wf.Terminaison);
        }

        [Fact]
        public void ToDomain_TerminaisonSansDateNiUpdate_UtiliseDateExpedition()
        {
            WorkflowRow row = RowValide();
            row.IdUserTerminaison = 42;
            row.DateTerminaison = null;
            row.DateUpdate = null;

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.Equal(new AuditInfo(42, row.DateExpedition), wf.Terminaison);
        }

        [Fact]
        public void ToDomain_DateTerminaisonSansUtilisateur_ResteEnCours()
        {
            WorkflowRow row = RowValide();
            row.IdUserTerminaison = null;
            row.DateTerminaison = T0.AddHours(3);

            Workflow wf = WorkflowMapper.ToDomain(row);

            Assert.False(wf.EstTermine);
        }
        // ------------------------------------------------------------


        // --- ToDomain : données corrompues en base ------------------

        [Theory]
        [InlineData("IdWorkflow")]
        [InlineData("NumeroDossier")]
        [InlineData("Sequence")]
        [InlineData("IdUserExpediteur")]
        [InlineData("DateExpedition")]
        [InlineData("IdUserAssigne")]
        [InlineData("Type")]
        [InlineData("Action")]
        [InlineData("DocumentSansId")]
        [InlineData("SansAssignation")]
        public void ToDomain_DonneeCorrompue_LeveInvalidData(string champ)
        {
            WorkflowRow row = RowValide();
            Corrompre(row, champ);

            Assert.Throws<InvalidDataException>(() => WorkflowMapper.ToDomain(row));
        }

        [Fact]
        public void ToDomain_DonneeCorrompue_ConserveLExceptionDOrigine()
        {
            WorkflowRow row = RowValide();
            row.NumeroDossier = 0;

            var ex = Assert.Throws<InvalidDataException>(() => WorkflowMapper.ToDomain(row));

            Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
            Assert.Contains("10", ex.Message); // le message cite l'id du workflow fautif
        }

        private static void Corrompre(WorkflowRow row, string champ)
        {
            switch (champ)
            {
                case "IdWorkflow": row.IdWorkflow = 0; break;
                case "NumeroDossier": row.NumeroDossier = 0; break;
                case "Sequence": row.Sequence = 100; break;
                case "IdUserExpediteur": row.IdUserExpediteur = 0; break;
                case "DateExpedition": row.DateExpedition = default; break;
                case "IdUserAssigne": row.IdUserAssigne = 0; break;
                case "Type": row.Type = 99; break;
                case "Action": row.Action = 99; break;
                case "DocumentSansId": row.Type = (int)WorkflowType.Document; row.IdDocument = 0; break;
                case "SansAssignation": row.IdUserAssigne = null; row.IdGroupeAssigne = null; break;
                default: throw new ArgumentException($"Cas inconnu : {champ}", nameof(champ));
            }
        }
    }
}
