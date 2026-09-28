using FDL.Core.Domain;
using FDL.Infra.Persistance;
using FDL.Loc.Domain;

namespace FDL.Infra.Tests
{
    public class RegistreMapperTests
    {
        private static readonly DateTime T0 = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);
        private static readonly ReferenceDossier Dossier = ReferenceDossier.DepuisExistant(2050123, 61);
        private static readonly int[] _listeCommunes = [1000, 1080, 1210];

        // Ligne valide par défaut : un logement en cours sur le dossier 2.050.123/61.
        // DateStatut ≠ DateCreation pour vérifier que les deux colonnes ne sont pas confondues.
        // Chaque test ne modifie que ce qui l'intéresse.
        private static RegistreRow RowValide() => new()
        {
            IdRegistre = 12,
            ReferenceDossier = 205012361,
            Type = (int)RegistreType.Logement,
            Statut = (int)RegistreStatut.EnCours,
            DateStatut = T0.AddDays(1),
            NbChambresMin = 1,
            NbChambresMax = 2,
            ListeCommunes = [.. _listeCommunes],
            SouhaiteAscenseur = true,
            IdUserCreation = 1,
            DateCreation = T0,
            Commentaire = "Commentaire test"
        };
        // ------------------------------------------------------------


        // --- Aller-retour Domain -> Row -> Domain -------------------

        [Theory]
        [InlineData("LogementEnCours")]
        [InlineData("PMRTermine")]
        [InlineData("CommerceRadieNotationHeritee")]
        [InlineData("ParkingSansCommentaire")]
        public void AllerRetour_DomainVersRowVersDomain_ConserveToutesLesDonnees(string cas)
        {
            Registre attendu = RegistreDeTest(cas);

            Registre obtenu = RegistreMapper.ToDomain(RegistreMapper.ToRow(attendu));

            Assert.Equal(attendu.IdRegistre, obtenu.IdRegistre);
            Assert.Equal(attendu.ReferenceDossier, obtenu.ReferenceDossier);
            Assert.Equal(attendu.Type, obtenu.Type);
            Assert.Equal(attendu.Statut, obtenu.Statut);
            Assert.Equal(attendu.DateStatut, obtenu.DateStatut);
            Assert.Equal(attendu.NbChambresMinimum, obtenu.NbChambresMinimum);
            Assert.Equal(attendu.NbChambresMaximum, obtenu.NbChambresMaximum);
            Assert.Equal(attendu.ListeCommunes, obtenu.ListeCommunes);
            Assert.Equal(attendu.SouhaiteAscenseur, obtenu.SouhaiteAscenseur);
            Assert.Equal(attendu.Auteur, obtenu.Auteur);
            Assert.Equal(attendu.Commentaire, obtenu.Commentaire);
        }

        private static Registre RegistreDeTest(string cas) => cas switch
        {
            "LogementEnCours" => Registre.Reconstituer(15, Dossier, RegistreType.Logement,
                RegistreStatut.EnCours, T0, 1, 2, [.. _listeCommunes], true, new AuditInfo(1, T0), "Logement"),

            // Statut changé après la création : DateStatut ≠ date de l'auteur
            "PMRTermine" => Registre.Reconstituer(16, Dossier, RegistreType.PMR,
                RegistreStatut.Termine, T0.AddDays(30), 0, 3, [1050], false, new AuditInfo(2, T0), "PMR"),

            // Dossier en notation héritée (séquence 0)
            "CommerceRadieNotationHeritee" => Registre.Reconstituer(17, ReferenceDossier.DepuisExistant(2050123, 0),
                RegistreType.Commerce, RegistreStatut.Radie, T0.AddDays(60), 0, 0, [1000, 1210], false,
                new AuditInfo(3, T0), "Commerce"),

            "ParkingSansCommentaire" => Registre.Reconstituer(18, Dossier, RegistreType.Parking,
                RegistreStatut.EnCours, T0, 0, 0, [1180], false, new AuditInfo(1, T0), null),

            _ => throw new ArgumentException($"Cas inconnu : {cas}", nameof(cas))
        };
        // ------------------------------------------------------------


        // --- ToRow --------------------------------------------------

        [Fact]
        public void ToRow_ReferenceDossier_EncodeeEnUnSeulEntier()
        {
            RegistreRow row = RegistreMapper.ToRow(RegistreMapper.ToDomain(RowValide()));

            Assert.Equal(205012361, row.ReferenceDossier);   // 2.050.123/61
        }

        [Fact]
        public void ToRow_NouveauRegistre_IdZeroPourLaBase()
        {
            // Un registre pas encore enregistré a l'id 0 : la base attribuera l'id (identity)
            Registre nouveau = Registre.PourLogement(Dossier, 1, 2, _listeCommunes, false, new AuditInfo(1, T0), null);

            RegistreRow row = RegistreMapper.ToRow(nouveau);

            Assert.Equal(0, row.IdRegistre);
            Assert.Equal((int)RegistreStatut.EnCours, row.Statut);
            Assert.Equal(T0, row.DateStatut);
            Assert.Equal(1, row.IdUserCreation);
            Assert.Equal(T0, row.DateCreation);
        }

        [Fact]
        public void ToRow_ListeCommunesModifieeDansLaRow_RegistreInchange()
        {
            Registre registre = RegistreMapper.ToDomain(RowValide());
            RegistreRow row = RegistreMapper.ToRow(registre);

            row.ListeCommunes[0] = 9999;

            Assert.Equal(_listeCommunes, registre.ListeCommunes);
        }

        [Fact]
        public void ToRow_AuditUpdate_NonRempli()
        {
            // L'audit update est géré en base : le mapper ne doit pas l'écraser
            RegistreRow row = RegistreMapper.ToRow(RegistreMapper.ToDomain(RowValide()));

            Assert.Null(row.IdUserUpdate);
            Assert.Null(row.DateUpdate);
        }
        // ------------------------------------------------------------


        // --- ToDomain : lignes valides ------------------------------

        [Fact]
        public void ToDomain_RowValide_DecodeReferenceEtAuteur()
        {
            Registre registre = RegistreMapper.ToDomain(RowValide());

            Assert.Equal(Dossier, registre.ReferenceDossier);
            Assert.Equal(new AuditInfo(1, T0), registre.Auteur);
            Assert.Equal(T0.AddDays(1), registre.DateStatut);   // pas la DateCreation
        }

        [Fact]
        public void ToDomain_ListeCommunesModifieeDansLaRow_RegistreInchange()
        {
            RegistreRow row = RowValide();
            Registre registre = RegistreMapper.ToDomain(row);

            row.ListeCommunes[0] = 9999;

            Assert.Equal(_listeCommunes, registre.ListeCommunes);
        }
        // ------------------------------------------------------------


        // --- ToDomain : données corrompues en base ------------------

        [Theory]
        [InlineData("IdRegistre")]
        [InlineData("ReferenceDossierZero")]
        [InlineData("ReferenceDossierNegative")]
        [InlineData("ReferenceDossierContrat")]
        [InlineData("Type")]
        [InlineData("Statut")]
        [InlineData("DateStatut")]
        [InlineData("IdUserCreation")]
        [InlineData("DateCreation")]
        [InlineData("NbChambresMinSuperieurMax")]
        [InlineData("NbChambresMaxTropGrand")]
        [InlineData("NbChambresNegatif")]
        [InlineData("ListeCommunesNull")]
        [InlineData("ListeCommunesVide")]
        [InlineData("ListeCommunesHorsBruxelles")]
        [InlineData("ListeCommunesDoublon")]
        public void ToDomain_DonneeCorrompue_LeveInvalidData(string champ)
        {
            RegistreRow row = RowValide();
            Corrompre(row, champ);

            Assert.Throws<InvalidDataException>(() => RegistreMapper.ToDomain(row));
        }

        [Fact]
        public void ToDomain_DonneeCorrompue_ConserveLExceptionDOrigine()
        {
            RegistreRow row = RowValide();
            row.ReferenceDossier = 0;

            var ex = Assert.Throws<InvalidDataException>(() => RegistreMapper.ToDomain(row));

            Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
            Assert.Contains("12", ex.Message);   // le message cite l'id du registre fautif
        }

        private static void Corrompre(RegistreRow row, string champ)
        {
            switch (champ)
            {
                case "IdRegistre": row.IdRegistre = 0; break;
                case "ReferenceDossierZero": row.ReferenceDossier = 0; break;
                case "ReferenceDossierNegative": row.ReferenceDossier = -205012361; break;
                case "ReferenceDossierContrat": row.ReferenceDossier = 205012301; break;   // séquence 01 = contrat
                case "Type": row.Type = 99; break;
                case "Statut": row.Statut = 0; break;
                case "DateStatut": row.DateStatut = default; break;
                case "IdUserCreation": row.IdUserCreation = 0; break;
                case "DateCreation": row.DateCreation = default; break;
                case "NbChambresMinSuperieurMax": row.NbChambresMin = 3; row.NbChambresMax = 2; break;
                case "NbChambresMaxTropGrand": row.NbChambresMax = Registre.NombreChambresMaximumAutorise + 1; break;
                case "NbChambresNegatif": row.NbChambresMin = -1; break;
                case "ListeCommunesNull": row.ListeCommunes = null!; break;
                case "ListeCommunesVide": row.ListeCommunes = []; break;
                case "ListeCommunesHorsBruxelles": row.ListeCommunes = [1050, 1300]; break;
                case "ListeCommunesDoublon": row.ListeCommunes = [1050, 1050]; break;
                default: throw new ArgumentException($"Cas inconnu : {champ}", nameof(champ));
            }
        }
    }
}
