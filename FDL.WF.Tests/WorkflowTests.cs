using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.WF.Tests
{
    public class WorkflowTests
    {
        private static readonly DateTime _dateTime = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);
        private static readonly AuditInfo _expediteur = new(1, _dateTime);
        private static readonly AuditInfo _destinataire = new(2, _dateTime.AddHours(2));
        private static readonly AuditInfo _otherDestinataire = new(5, _dateTime.AddMinutes(5));

        public static Workflow NouvelleNote() =>
            Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");


        [Fact]
        public void CreationWFPourNote()
        {
            var wf = NouvelleNote();
            Assert.Equal(WorkflowAction.ALire, wf.Action);
            Assert.Equal(WorkflowType.Note, wf.Type);
            Assert.Null(wf.Terminaison);
            Assert.False(wf.EstTermine);
        }

        [Fact]
        public void CreationWFPourDocument()
        {
            string msg = "Message test";
            int idDoc = 1534;
            var wf = Workflow.PourDocument(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test",
                WorkflowAction.AVerifier,
                idDoc);
            Assert.Equal(WorkflowAction.AVerifier, wf.Action);
            Assert.Equal(WorkflowType.Document, wf.Type);
            Assert.False(wf.EstTermine);
            Assert.Null(wf.Terminaison);
            Assert.Equal(msg, wf.Message);
            Assert.Equal(idDoc, wf.IdDocument);
        }

        [Fact]
        public void CreationWFPourDocumentSansId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Workflow.PourDocument(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test",
                WorkflowAction.ALire,
                0) // idDocument = 0
            );
        }

        [Fact]
        public void CreationWFPourTache()
        {
            string msg = "Message test";
            var wf = Workflow.PourTache(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                msg);
            Assert.Equal(WorkflowAction.AValider, wf.Action);
            Assert.Equal(WorkflowType.Tache, wf.Type);
            Assert.Null(wf.Terminaison);
            Assert.False(wf.EstTermine);
            Assert.Equal(msg, wf.Message);
        }



        [Fact]
        public void TerminerUnWF()
        {
            var wf = Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");
            wf.Terminer(_destinataire);

            Assert.NotNull(wf.Terminaison);
            Assert.Equal(_destinataire, wf.Terminaison);
            Assert.True(wf.EstTermine);
        }

        [Fact]
        public void TerminerUnWFDejaTermine()
        {
            var wf = Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");
            wf.Terminer(_destinataire);

            // on essaie de le terminer une seconde fois
            Assert.Throws<InvalidOperationException>(() =>
                wf.Terminer(_otherDestinataire)
            );
            Assert.Equal(_destinataire, wf.Terminaison);
        }

        [Fact]
        public void ReassignerUnWF()
        {
            var wf = Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");

            wf.Reassigner(WorkflowAssignation.PourUtilisateur(_otherDestinataire.IdUser));
            Assert.Equal(_otherDestinataire.IdUser, wf.Assignation.IdUser);
            Assert.Null(wf.Assignation.IdGroup);
            Assert.True(wf.Assignation.EstUtilisateur);
            Assert.False(wf.Assignation.EstGroupe);
        }

        [Fact]
        public void ReassignerUnWFDejaTermine()
        {
            var wf = Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");
            wf.Terminer(_destinataire);
            // on essaie de réassigner un wf terminé
            Assert.Throws<InvalidOperationException>(() =>
                wf.Reassigner(WorkflowAssignation.PourUtilisateur(_otherDestinataire.IdUser))
            );
            Assert.Equal(_destinataire.IdUser, wf.Assignation.IdUser);
        }

        [Fact]
        public void ReassignerUtilisateurVersGroupe()
        {
            var wf = Workflow.PourNote(
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                WorkflowAssignation.PourUtilisateur(_destinataire.IdUser),
                "Message test");
            Assert.Equal(_destinataire.IdUser, wf.Assignation.IdUser);
            Assert.True(wf.Assignation.EstUtilisateur);
            Assert.False(wf.Assignation.EstGroupe);
            Assert.Null(wf.Assignation.IdGroup);
            // nouvelle assignation
            int idGroupe = 28;
            wf.Reassigner(WorkflowAssignation.PourGroupe(idGroupe));
            Assert.True(wf.Assignation.EstGroupe);
            Assert.False(wf.Assignation.EstUtilisateur);
            Assert.Equal(idGroupe, wf.Assignation.IdGroup);
            Assert.Null(wf.Assignation.IdUser);
        }


        private static Workflow Reconstituer(
            int id = 10,
            WorkflowType type = WorkflowType.Note,
            WorkflowAction action = WorkflowAction.ALire,
            int idDocument = 0)
            => Workflow.Reconstituer(id, ReferenceDossier.DepuisExistant(1500123, 1), _expediteur,
                WorkflowAssignation.PourUtilisateur(2), "Message test", type, action, idDocument, null);

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReconstituerUnWF_IdInvalide(int idWorkflow)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Reconstituer(id: idWorkflow)
            );
        }


        [Theory]
        [InlineData((WorkflowType)99, WorkflowAction.ALire, 0)]        // type inconnu
        [InlineData(WorkflowType.Note, (WorkflowAction)99, 0)]         // action inconnue
        [InlineData(WorkflowType.Document, WorkflowAction.ASigner, 0)] // document sans id
        public void ReconstituerUnWF_DonneesIncoherentes(WorkflowType type , WorkflowAction action, int idDocument)
        {
            Assert.Throws<InvalidDataException>(() =>
                Reconstituer(type: type, action: action, idDocument: idDocument)
            );
        }

        [Fact]
        public void ReconstitionValide()
        {
            AuditInfo terminaison = new(2, new DateTime(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc));
            var wf = Workflow.Reconstituer(10,
                ReferenceDossier.DepuisExistant(1500123, 1),
                _expediteur,
                 WorkflowAssignation.PourUtilisateur(2),
                 "Message test",
                 WorkflowType.Document,
                 WorkflowAction.ASigner,
                 502,
                 terminaison);
            Assert.Equal(terminaison, wf.Terminaison);
            Assert.True(wf.EstTermine);
        }
    }
}
