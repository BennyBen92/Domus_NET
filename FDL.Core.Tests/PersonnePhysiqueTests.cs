using FDL.Core.Domain;
using System.Globalization;

namespace FDL.Core.Tests
{
    public class PersonnePhysiqueTests
    {
        private static readonly DateOnly Aujourdhui = new(2026, 6, 1);
        private static readonly DateOnly Anniversaire = new(1991, 6, 1);
        private static readonly NumeroNational NissHomme = NumeroNational.Parse("91060112354");

        private static PersonnePhysique CreerPersonne(
            string nom = "Doe",
            string? prenom = "John",
            DateOnly? dateNaissance = null,
            NumeroNational? numeroNational = null,
            Sexe sexe = Sexe.Masculin) =>
            PersonnePhysique.Creer(
                nom: nom,
                prenom: prenom,
                dateNaissance: dateNaissance ?? Anniversaire,
                numeroNational: numeroNational ?? NissHomme,
                sexe: sexe);

        private static PersonnePhysique ReconstituerPersonne(
            int idPersonnePhysique = 1,
            string nom = "Doe",
            string? prenom = "John",
            DateOnly? dateNaissance = null,
            NumeroNational? numeroNational = null,
            Sexe sexe = Sexe.Masculin) =>
            PersonnePhysique.Reconstituer(
                idPersonnePhysique: idPersonnePhysique,
                nom: nom,
                prenom: prenom,
                dateNaissance: dateNaissance ?? Anniversaire,
                numeroNational: numeroNational ?? NissHomme,
                sexe: sexe);

        [Theory]
        [InlineData(null, "Doe")]
        [InlineData("", "Doe")]
        [InlineData("     ", "Doe")]
        [InlineData("John", "John Doe")]
        public void NomComplet_Retourne_JohnDoe(string? prenom, string expected)
        {
            PersonnePhysique pers = CreerPersonne(prenom: prenom);
            Assert.Equal(expected, pers.NomComplet());
        }

        [Theory]
        [InlineData(0, 35)]
        [InlineData(-1, 34)]
        [InlineData(1, 35)]
        public void CalculerAge_SelonDateReference_RetourneAgeAttendu(int joursAjoute, int ageAttendu)
        {
            DateOnly dateRef = new(Aujourdhui.Year, Anniversaire.Month, Anniversaire.Day);
            var pers = CreerPersonne(dateNaissance: Anniversaire);
            Assert.Equal(ageAttendu, pers.Age(dateRef.AddDays(joursAjoute)));
        }
        [Fact]
        public void CalculerAge_SelonAnniversaire_Retourne_0()
        {
            var pers = CreerPersonne(dateNaissance: Anniversaire);
            Assert.Equal(0, pers.Age(Anniversaire));
        }
        [Fact]
        public void CalculerAge_DateAnterieurAnniversaire_LeveArgumentOutOfRangeException()
        {
            var pers = CreerPersonne(dateNaissance: Anniversaire);
            DateOnly dateAnterieur = Anniversaire.AddDays(-1);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                pers.Age(dateAnterieur)
            );
        }


        // Création
        [Fact]
        public void CreerPersonnePhysique_Valide_AucuneException()
        {
            PersonnePhysique pers = CreerPersonne();
            Assert.NotNull(pers);
            Assert.Equal("John", pers.Prenom);
            Assert.Equal("Doe", pers.Nom);
            Assert.Equal(NissHomme, pers.NumeroNational);
            Assert.Equal(Anniversaire, pers.DateNaissance);
            Assert.Equal(Sexe.Masculin, pers.Sexe);
            Assert.Equal(0, pers.IdPersonnePhysique);
        }

        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        public void CreerPersonnePhysique_NomVideOuEspace_LeveArgumentException(string nom)
        {
            var ex = Assert.Throws<ArgumentException>(() => CreerPersonne(nom: nom));
            Assert.Equal("nom", ex.ParamName);
        }
        [Fact]
        public void CreerPersonnePhysique_NomNull_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreerPersonne(nom: null!));
            Assert.Equal("nom", ex.ParamName);
        }
        [Fact]
        public void CreerPersonnePhysique_DateNaissanceFuture_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreerPersonne(dateNaissance: new(2099, 12, 1)));
            Assert.Equal("dateNaissance", ex.ParamName);
        }
        [Fact]
        public void CreerPersonnePhysique_DateNaissanceParDefaut_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreerPersonne(dateNaissance: default(DateOnly)));
            Assert.Equal("dateNaissance", ex.ParamName);
        }
        [Fact]
        public void CreerPersonnePhysique_NumeroNationalNull_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                PersonnePhysique.Creer("Doe", "John", Anniversaire, null!, Sexe.Masculin));
            Assert.Equal("numeroNational", ex.ParamName);
        }
        [Fact]
        public void CreerPersonnePhysique_SexeInvalide_LeveInvalidDataException()
        {
            // sexe contrôlé avant la cohérence du NISS
            Assert.Throws<InvalidDataException>(() => CreerPersonne(sexe: (Sexe)99));
        }

        // Reconstitution
        [Fact]
        public void ReconstituerPersonnePhysique_Valide_AucuneException()
        {
            PersonnePhysique pers = ReconstituerPersonne();
            Assert.NotNull(pers);
            Assert.Equal("John", pers.Prenom);
            Assert.Equal("Doe", pers.Nom);
            Assert.Equal(NissHomme, pers.NumeroNational);
            Assert.Equal(Anniversaire, pers.DateNaissance);
            Assert.Equal(Sexe.Masculin, pers.Sexe);
            Assert.Equal(1, pers.IdPersonnePhysique);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReconstituerPersonnePhysique_IdPersonnePhysiqueInvalide_LeveArgumentException(int idPersPhys)
        {
            var ex = Assert.Throws<ArgumentException>(() => ReconstituerPersonne(idPersonnePhysique: idPersPhys));
            Assert.Equal("idPersonnePhysique", ex.ParamName);
        }
        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        public void ReconstituerPersonnePhysique_NomVideOuEspace_LeveArgumentException(string nom)
        {
            var ex = Assert.Throws<ArgumentException>(() => ReconstituerPersonne(nom: nom));
            Assert.Equal("nom", ex.ParamName);
        }
        [Fact]
        public void ReconstituerPersonnePhysique_NomNull_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreerPersonne(nom: null!));
            Assert.Equal("nom", ex.ParamName);
        }
        [Fact]
        public void ReconstituerPersonnePhysique_DateNaissanceFuture_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => ReconstituerPersonne(dateNaissance: new(2099, 12, 1)));
            Assert.Equal("dateNaissance", ex.ParamName);
        }
        [Fact]
        public void ReconstituerPersonnePhysique_DateNaissanceParDefaut_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => ReconstituerPersonne(dateNaissance: default(DateOnly)));
            Assert.Equal("dateNaissance", ex.ParamName);
        }
        [Fact]
        public void ReconstituerPersonnePhysique_NumeroNationalNull_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                PersonnePhysique.Reconstituer(1, "Doe", "John", Anniversaire, null!, Sexe.Masculin));
            Assert.Equal("numeroNational", ex.ParamName);
        }
        [Fact]
        public void ReconstituerPersonnePhysique_SexeInvalide_LeveInvalidDataException() =>
            Assert.Throws<InvalidDataException>(() => ReconstituerPersonne(sexe: (Sexe)99));

        // Cohérences
        [Theory]
        [InlineData("91060112453", "1991-06-01", Sexe.Feminin)]      // femme cohérente
        [InlineData("91060112354", "1991-06-01", Sexe.NonBinaire)]   // NonBinaire : parité non contrôlée
        [InlineData("00000000097", "1985-12-31", Sexe.Masculin)]     // temporaire, tout est accepté
        [InlineData("00000000097", "2020-02-29", Sexe.Feminin)]
        [InlineData("00000000097", "1991-06-01", Sexe.NonBinaire)]
        public void CreerPersonnePhysique_NumeroNationalCoherentOuTemporaire_EstAccepte(string nissBrut, string date, Sexe sexe)
        {
            NumeroNational nn = NumeroNational.Parse(nissBrut);
            PersonnePhysique pers = CreerPersonne(dateNaissance: DateOnly.Parse(date, CultureInfo.InvariantCulture),
                                                  numeroNational: nn, sexe: sexe);
            Assert.Equal(nn, pers.NumeroNational);
        }

        [Theory]
        [InlineData("91060112453", "1991-06-01", Sexe.Masculin)]   // parité : numéro de femme, sexe masculin
        [InlineData("91060112354", "1991-06-02", Sexe.Masculin)]   // jour différent
        [InlineData("91060112354", "1991-07-01", Sexe.Masculin)]   // mois différent
        [InlineData("91060112354", "1992-06-01", Sexe.Masculin)]   // année différente
        [InlineData("05031512367", "1905-03-15", Sexe.Masculin)]   // siècle : la clé 67 n'existe que pour 2000+
        public void CreerPersonnePhysique_NumeroNationalIncoherent_LeveArgumentException(string nissBrut, string date, Sexe sexe)
        {
            NumeroNational nn = NumeroNational.Parse(nissBrut);
            DateOnly naissance = DateOnly.Parse(date, CultureInfo.InvariantCulture);

            var ex = Assert.Throws<ArgumentException>(() =>
                CreerPersonne(dateNaissance: naissance, numeroNational: nn, sexe: sexe));
            Assert.Equal("numeroNational", ex.ParamName);
        }

        [Theory]
        [InlineData("91060112453", "1991-06-01", Sexe.Feminin)]      // femme cohérente
        [InlineData("91060112354", "1991-06-01", Sexe.NonBinaire)]   // NonBinaire : parité non contrôlée
        [InlineData("00000000097", "1985-12-31", Sexe.Masculin)]     // temporaire, tout est accepté
        [InlineData("00000000097", "2020-02-29", Sexe.Feminin)]
        [InlineData("00000000097", "1991-06-01", Sexe.NonBinaire)]
        public void ReconstituerPersonnePhysique_NumeroNationalCoherentOuTemporaire_EstAccepte(string nissBrut, string date, Sexe sexe)
        {
            NumeroNational nn = NumeroNational.Parse(nissBrut);
            PersonnePhysique pers = ReconstituerPersonne(dateNaissance: DateOnly.Parse(date, CultureInfo.InvariantCulture),
                                                        numeroNational: nn, sexe: sexe);
            Assert.Equal(nn, pers.NumeroNational);
        }
        [Theory]
        [InlineData("91060112453", "1991-06-01", Sexe.Masculin)]   // parité : numéro de femme, sexe masculin
        [InlineData("91060112354", "1991-06-02", Sexe.Masculin)]   // jour différent
        [InlineData("91060112354", "1991-07-01", Sexe.Masculin)]   // mois différent
        [InlineData("91060112354", "1992-06-01", Sexe.Masculin)]   // année différente
        [InlineData("05031512367", "1905-03-15", Sexe.Masculin)]   // siècle : la clé 67 n'existe que pour 2000+
        public void ReconstituerPersonnePhysique_NumeroNationalIncoherent_LeveArgumentException(string nissBrut, string date, Sexe sexe)
        {
            NumeroNational nn = NumeroNational.Parse(nissBrut);
            DateOnly naissance = DateOnly.Parse(date, CultureInfo.InvariantCulture);

            var ex = Assert.Throws<ArgumentException>(() =>
                ReconstituerPersonne(dateNaissance: naissance, numeroNational: nn, sexe: sexe));
            Assert.Equal("numeroNational", ex.ParamName);
        }



    }
}
