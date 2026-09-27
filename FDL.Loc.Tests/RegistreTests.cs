using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Loc.Tests
{
    public class RegistreTests
    {
        private static readonly DateTime _dateTime = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _dateCloture = new(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        private static readonly AuditInfo _auteur = new(1, _dateTime);
        private static readonly ReferenceDossier _referenceDemande = ReferenceDossier.DepuisExistant(2050123, 61);
        private static readonly int[] _listeCommunes = [1000, 1080, 1210];
        private static Registre NouveauRegistreLogement(ReferenceDossier refDossier) =>
            Registre.PourLogement(
                referenceDossier: refDossier,
                nbChambresMin: 1,
                nbChambresMax: 2,
                listeCommunes: _listeCommunes,
                souhaiteAscenseur: false,
                auteur: _auteur,
                commentaire: "un registre de test");
        // ------------------------------------------------------------


        // Référence dossier
        [Fact]
        public void Registre_AvecReferenceDeDemande_EstAccepte()
        {
            Assert.Null(Record.Exception(() =>
                {
                    // Séquence 61 est une demande
                    Registre r = NouveauRegistreLogement(_referenceDemande);
                    Assert.Equal(_referenceDemande, r.RefDossier);
                }
            ));
            Assert.Null(Record.Exception(() =>
            {
                // Séquence 00 est une demande (par le passé)
                Registre r = NouveauRegistreLogement(ReferenceDossier.DepuisExistant(2055020, 0));
            }
            ));
        }
        [Fact]
        public void Registre_AvecReferenceDeContrat_LeveUneException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // Séquence 01 est un contrat, pas une demande
                Registre r = NouveauRegistreLogement(ReferenceDossier.DepuisExistant(2050123, 1));
            });
        }
        // ------------------------------------------------------------

        // Méthodes de validation
        [Fact]
        public void ListeCommunesValide_ListeValide_AucuneException()
        {
            Assert.Null(Record.Exception(() =>
                Registre.ListeCommunesValide(_listeCommunes)
            ));
        }

        [Fact]
        public void ListeCommunesValide_ListeNull_LeveArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Registre.ListeCommunesValide(null!)
            );
        }

        [Theory]
        [InlineData((int[])[])]                 // liste vide
        [InlineData((int[])[1050, 1200, 1050])] // doublon
        public void ListeCommunesValide_ListeVideOuDoublon_LeveArgumentException(int[] liste)
        {
            Assert.Throws<ArgumentException>(() =>
                Registre.ListeCommunesValide(liste)
            );
        }

        [Theory]
        [InlineData((int[])[0])]            // zéro
        [InlineData((int[])[999])]          // juste sous la borne
        [InlineData((int[])[1211])]         // juste au-dessus
        [InlineData((int[])[1050, 1211])]   // un seul code invalide suffit
        public void ListeCommunesValide_CodeHorsBruxelles_LeveArgumentOutOfRange(int[] liste)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Registre.ListeCommunesValide(liste)
            );
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(2, 4)]
        public void IntervalNombresChambresValide_Valide_AucuneException(int min, int max)
        {
            Assert.Null(Record.Exception(() => Registre.NombresChambresSouhaitesValides(min, max)));
        }
        [Theory]
        [InlineData(-1, 1)]
        [InlineData(-3, -1)]
        [InlineData(0, -1)]
        public void IntervalNombresChambresInvalide_Negative_LeveArgumentOutOfRangeException(int min, int max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Registre.NombresChambresSouhaitesValides(min, max));
        }
        [Theory]
        [InlineData(2, 1)]
        [InlineData(1, 0)]
        public void IntervalNombresChambresInvalide_MinMaxInverse_LeveArgumentOutOfRangeException(int min, int max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Registre.NombresChambresSouhaitesValides(min, max));
        }
        [Theory]
        // (7, 7)
        [InlineData(Registre.NombreChambresMaximumAutorise + 1, Registre.NombreChambresMaximumAutorise + 1)]
        // (5, 7)
        [InlineData(5, Registre.NombreChambresMaximumAutorise + 1)]
        public void IntervalNombresChambresInvalide_DepassementNombreMaximumAutorise_LeveArgumentOutOfRangeException(int min, int max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Registre.NombresChambresSouhaitesValides(min, max));
        }
        // ------------------------------------------------------------


        // Fabriques
        [Fact]
        public void RegistrePourLogement_ParDefaut_EstEnCoursEtTypeLogement()
        {
            Registre r = Registre.PourLogement(_referenceDemande, 0, 0, _listeCommunes, false, _auteur, "");
            Assert.Equal(RegistreType.Logement, r.Type);
            Assert.True(r.EstLogement);
            Assert.True(r.EstEnCours);
        }
        [Fact]
        public void RegistrePourPMR_ParDefaut_EstEnCoursEtTypePMR()
        {
            Registre r = Registre.PourPMR(_referenceDemande, 0, 0, _listeCommunes, false, _auteur, "");
            Assert.Equal(RegistreType.PMR, r.Type);
            Assert.True(r.EstPMR);
            Assert.True(r.EstEnCours);
        }
        [Fact]
        public void RegistrePourCommerce_ParDefaut_EstEnCoursEtTypeCommerce()
        {
            Registre r = Registre.PourCommerce(_referenceDemande, _listeCommunes, _auteur, "");
            Assert.Equal(RegistreType.Commerce, r.Type);
            Assert.True(r.EstCommerce);
            Assert.True(r.EstEnCours);
        }
        [Fact]
        public void RegistrePourParking_ParDefaut_EstEnCoursEtTypeParking()
        {
            Registre r = Registre.PourParking(_referenceDemande, _listeCommunes, _auteur, "");
            Assert.Equal(RegistreType.Parking, r.Type);
            Assert.True(r.EstParking);
            Assert.True(r.EstEnCours);
        }


        [Fact]
        public void RegistrePourLogement_ListeCommunesModifieeApresCreation_RegistreInchange()
        {
            int[] communes = [1050, 1080];
            Registre r = Registre.PourLogement(_referenceDemande, 1, 2, communes, false, _auteur, null);

            communes[0] = 9999;

            Assert.Equal([1050, 1080], r.ListeCommunes);
        }
        // ------------------------------------------------------------

        // Reconstitution
        private static Registre Reconstituer(
            int id = 12,
            RegistreType type = RegistreType.Logement,
            RegistreStatut statut = RegistreStatut.EnCours,
            int nbChambresMin = 0,
            int nbChambresMax = 0,
            bool souhaiteAscenseur = false,
            DateTime? dateStatut = null
            ) => Registre.Reconstituer(
                idRegistre: id,
                referenceDossier: _referenceDemande,
                type: type,
                statut: statut,
                dateStatut: dateStatut ?? _dateTime,
                nbChambresMin: nbChambresMin,
                nbChambresMax: nbChambresMax,
                listeCommunes: _listeCommunes,
                souhaiteAscenseur: souhaiteAscenseur,
                auteur: _auteur,
                commentaire: ""
            );

        [Fact]
        public void Reconstitution_ParDefaut_AucuneException()
        {
            var registre = Registre.Reconstituer(
                idRegistre: 12,
                referenceDossier: _referenceDemande,
                type: RegistreType.Logement,
                statut: RegistreStatut.EnCours,
                dateStatut: _dateTime,
                nbChambresMin: 1,
                nbChambresMax: 2,
                listeCommunes: _listeCommunes,
                souhaiteAscenseur: true,
                auteur: _auteur,
                commentaire: "Commentaire test"
            );
            Assert.Equal(_auteur, registre.Auteur);
            Assert.Equal(_referenceDemande, registre.RefDossier);
            Assert.Equal(_listeCommunes, registre.ListeCommunes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Reconstitution_IdInvalide_LeveArgumentOutOfRangeException(int idRegistre)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Reconstituer(id: idRegistre)
            );
        }

        [Theory]
        [InlineData((RegistreType)99, RegistreStatut.EnCours)]
        [InlineData(RegistreType.Logement, (RegistreStatut)0)]
        public void Reconstitution_DonneesIncoherentes_LeveInvalidDataException(RegistreType type, RegistreStatut statut)
        {
            Assert.Throws<InvalidDataException>(() =>
                Reconstituer(type: type, statut: statut)
            );
        }

        [Fact]
        public void Reconstitution_DateStatutParDefaut_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                Reconstituer(dateStatut: default(DateTime))
            );
            Assert.Equal("dateStatut", ex.ParamName);
        }
        // ------------------------------------------------------------

        // Modificateurs
        private static void Cloturer(Registre r, string action, DateTime date)
        {
            switch (action)
            {
                case "Terminer":
                    r.Terminer(date);
                    break;
                case "Radier":
                    r.Radier(date);
                    break;
                default:
                    throw new ArgumentException($"Action de test inconnue : {action}.");
            }
        }

        [Theory]
        [InlineData("Terminer", RegistreStatut.Termine)]
        [InlineData("Radier", RegistreStatut.Radie)]
        public void Cloturer_RegistreEnCours_ChangeStatutEtDate(string action, RegistreStatut attendu)
        {
            // Arrange
            Registre r = NouveauRegistreLogement(_referenceDemande);
            // Act
            Cloturer(r, action, _dateCloture);
            // Assert
            Assert.Equal(attendu, r.Statut);
            Assert.Equal(_dateCloture, r.DateStatut);
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void Cloturer_DateParDefaut_LeveArgumentException(string action)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);

            var ex = Assert.Throws<ArgumentException>(() => Cloturer(r, action, default));

            Assert.Equal("date", ex.ParamName);   // la bonne règle a sauté
            Assert.True(r.EstEnCours);            // rien n'a changé
        }

        [Theory]
        [InlineData("Terminer", "Terminer")]
        [InlineData("Terminer", "Radier")]
        [InlineData("Radier", "Terminer")]
        [InlineData("Radier", "Radier")]
        public void Cloturer_RegistreDejaCloture_LeveInvalidOperation(string premiere, string seconde)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            Cloturer(r, premiere, _dateCloture);
            RegistreStatut statutAvant = r.Statut;

            Assert.Throws<InvalidOperationException>(() => Cloturer(r, seconde, _dateCloture.AddDays(1)));

            Assert.Equal(statutAvant, r.Statut);
            Assert.Equal(_dateCloture, r.DateStatut);
        }
        // ------------------------------------------------------------


        [Fact]
        public void ChangeListeCommunes_RegistreEnCours_AucuneExceptionEtListeModifiee()
        {
            int[] nouvelleListe = [1000];
            Registre r = NouveauRegistreLogement(_referenceDemande);
            Assert.Null(Record.Exception(() =>
                r.ChangeListeCommunes(nouvelleListe)
            ));
            Assert.Equal(nouvelleListe, r.ListeCommunes);
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void ChangeListeCommunes_RegistreCloture_LeveInvalidOperationException(string action)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            var listeOrginale = Array.AsReadOnly([.. r.ListeCommunes]);
            Cloturer(r, action, _dateCloture);

            Assert.Throws<InvalidOperationException>(() =>
                r.ChangeListeCommunes([1000])
            );
            Assert.Equal(listeOrginale, r.ListeCommunes);
        }


        [Fact]
        public void ChangeNombresChambresSouhaites_RegistreEnCours_DonneesModifiees()
        {
            int min = 2;
            int max = 3;
            Registre r = NouveauRegistreLogement(_referenceDemande);
            Assert.Null(Record.Exception(() =>
                r.ChangeNombresChambresSouhaites(min, max)
            ));
            Assert.Equal(min, r.NbChambresMinimum);
            Assert.Equal(max, r.NbChambresMaximum);
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void ChangeNombresChambresSouhaites_RegistreCloture_LeveInvalidOperationException(string action)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            int minOriginal = r.NbChambresMinimum;
            int maxOriginal = r.NbChambresMaximum;
            Cloturer(r, action, _dateCloture);

            Assert.Throws<InvalidOperationException>(() =>
                r.ChangeNombresChambresSouhaites(2, 3)
            );
            Assert.Equal(minOriginal, r.NbChambresMinimum);
            Assert.Equal(maxOriginal, r.NbChambresMaximum);
        }


        [Fact]
        public void ChangeType_RegistreEnCours_TypeModifie()
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            Assert.Null(Record.Exception(() =>
                r.ChangeType(RegistreType.PMR)
            ));
            Assert.Equal(RegistreType.PMR, r.Type);
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void ChangeType_RegistreCloture_LeveInvalidOperationException(string action)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            RegistreType typeOriginal = r.Type;
            Cloturer(r, action, _dateCloture);

            Assert.Throws<InvalidOperationException>(() =>
                r.ChangeType(RegistreType.Parking)
            );
            Assert.Equal(typeOriginal, r.Type);
        }
        [Theory]
        [InlineData((RegistreType)0)]
        [InlineData((RegistreType)99)]
        public void ChangeType_TypeInvalide_ArgumentOutOfRangeException(RegistreType type)
        {
            Registre r = NouveauRegistreLogement(_referenceDemande);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                r.ChangeType(type)
            );
        }

    }
}
