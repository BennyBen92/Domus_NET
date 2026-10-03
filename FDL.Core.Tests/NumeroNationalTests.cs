using FDL.Core.Domain;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FDL.Core.Tests
{
    public class NumeroNationalTests
    {
        // Numéro temporaire
        [Fact]
        public void EstTemporaire_NumeroTemporaire_RetourneVrai() =>
            Assert.True(NumeroNational.Parse("00000000097").EstTemporaire);

        [Fact]
        public void EstTemporaire_NumeroNonTemporaire_RetourneFauxEtPasEgalAuTemporaire() =>
            Assert.False(NumeroNational.Parse("90010112395").EstTemporaire);
        // ------------------------------------------------------------

        // Egalité
        [Fact]
        public void Egalite_NumeroTemporaireFormate_EstEgalAuTemporaire() =>
            Assert.Equal(NumeroNational.Temporaire, NumeroNational.Parse("00.00.00-000.97"));
        [Fact]
        public void Egalite_NumeroFormate_EgalAuNumeroSansFormat() =>
            Assert.Equal(NumeroNational.Parse("90010112395"), NumeroNational.Parse("90.01.01-123.95"));
        // ------------------------------------------------------------

        // Parse
        [Theory]
        [InlineData("90010112395", "90010112395")]          // nominal
        [InlineData("90.01.01-123.95", "90010112395")]      // séparateurs
        [InlineData(" 90 01 01 123 95 ", "90010112395")]    // espaces
        [InlineData("00010112476", "00010112476")]          // clé avec préfixe 2 (années 2000)
        [InlineData("05031512367", "05031512367")]          // clé avec préfixe 2 (années 2005)
        [InlineData("85462012326", "85462012326")]          // BIS +40
        [InlineData("85262012380", "85262012380")]          // BIS +20
        [InlineData("00000000097", "00000000097")]          // temporaire
        [InlineData("00.00.00-000.97", "00000000097")]      // temporaire formaté
        public void Parse_ParseValide_RetourneValeurAttendue(string entree, string valeurAttendue)
        {
            NumeroNational result = NumeroNational.Parse(entree);
            Assert.NotNull(result);
            Assert.Equal(valeurAttendue, result.ToStringBrute());
        }

        [Fact]
        public void Parse_EntreeNulleParseInvalide_LeveArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => NumeroNational.Parse(null!));
            Assert.Equal("numeroNational", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        [InlineData("9001011239")]      // 10 chiffres
        [InlineData("900101123950")]
        [InlineData("9001A0112395")]    // caractères non numériques
        [InlineData("ABCDEFGHIJK")]
        [InlineData("90010112396")]     // clé fausse (+1)
        [InlineData("90130112382")]     // mois 13
        [InlineData("90190112327")]     // mois 19 (au bord min de la règle +20)
        [InlineData("90530112371")]     // mois 53 (au bord max de la règle +40)
        [InlineData("90013212338")]     // jour 32
        public void Parse_EntreeInvalideParseInvalide_LeveArgumentException(string entree)
        {
            var ex = Assert.Throws<ArgumentException>(() => NumeroNational.Parse(entree));
            Assert.Equal("numeroNational", ex.ParamName);
        }
        // ------------------------------------------------------------

        // TryParse
        [Theory]
        [InlineData("90010112395")]          // nominal
        [InlineData("90.01.01-123.95")]      // séparateurs
        [InlineData(" 90 01 01 123 95 ")]    // espaces
        [InlineData("00010112476")]          // clé avec préfixe 2 (années 2000)
        [InlineData("05031512367")]          // clé avec préfixe 2 (années 2005)
        [InlineData("85462012326")]          // BIS +40
        [InlineData("85262012380")]          // BIS +20
        [InlineData("00000000097")]          // temporaire
        [InlineData("00.00.00-000.97")]      // temporaire formaté
        public void TryParse_TryParseValide_RetourneVraiEtResultatNonNull(string entree)
        {
            Assert.True(NumeroNational.TryParse(entree, out NumeroNational? result));
            Assert.NotNull(result);
        }

        [Fact]
        public void TryParse_EntreeNulleTryParseInvalide_RetourneFauxEtResultatEstNull()
        {
            Assert.False(NumeroNational.TryParse(null, out NumeroNational? result));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        [InlineData("9001011239")]      // 10 chiffres
        [InlineData("900101123950")]
        [InlineData("9001A0112395")]    // caractères non numériques
        [InlineData("ABCDEFGHIJK")]
        [InlineData("90010112396")]     // clé fausse (+1)
        [InlineData("90130112382")]     // mois 13
        [InlineData("90190112327")]     // mois 19 (au bord min de la règle +20)
        [InlineData("90530112371")]     // mois 53 (au bord max de la règle +40)
        [InlineData("90013212338")]     // jour 32
        public void TryParse_EntreeInvalideTryParseInvalide_RetourneFauxEtResultatNull(string entree)
        {
            Assert.False(NumeroNational.TryParse(entree, out NumeroNational? result));
            Assert.Null(result);
        }
        // ------------------------------------------------------------

        // BIS
        [Fact]
        public void EstNumeroBISAvecSexeInconnu_NumeroBISValideAvecSexeInconnu_RetourneVrai() =>
            Assert.True(NumeroNational.Parse("85262012380").EstNumeroBISAvecSexeInconnu);
        [Fact]
        public void EstNumeroBISAvecSexeInconnu_NumeroNormalValide_RetourneFaux() =>
            Assert.False(NumeroNational.Parse("90010112395").EstNumeroBISAvecSexeInconnu);
        [Fact]
        public void EstNumeroBISAvecSexeInconnu_NumeroBISValideAvecSexeConnu_RetourneFaux() =>
            Assert.False(NumeroNational.Parse("85462012326").EstNumeroBISAvecSexeInconnu);

        [Fact]
        public void EstNumeroBISAvecSexeConnu_NumeroBISValideAvecSexeInconnu_RetourneFaux() =>
            Assert.False(NumeroNational.Parse("85262012380").EstNumeroBISAvecSexeConnu);
        [Fact]
        public void EstNumeroBISAvecSexeConnu_NumeroNormalValide_RetourneFaux() =>
            Assert.False(NumeroNational.Parse("90010112395").EstNumeroBISAvecSexeConnu);
        [Fact]
        public void EstNumeroBISAvecSexeConnu_NumeroBISValideAvecSexeConnu_RetourneVrai() =>
            Assert.True(NumeroNational.Parse("85462012326").EstNumeroBISAvecSexeConnu);
        // ------------------------------------------------------------

        // Mois réel
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(12, 12)]
        [InlineData(21, 1)]
        [InlineData(32, 12)]
        [InlineData(41, 1)]
        [InlineData(52, 12)]
        [InlineData(13, 13)]     // fonctionne avec mois 13, car pas de validation
        [InlineData(53, 13)]     // fonctionne avec mois 13, car pas de validation
        public void MoisReel_MoisValide_RetourneValeurAttendue(int mois, int moisReel) =>
            Assert.Equal(moisReel, NumeroNational.MoisReel(mois));
        // ------------------------------------------------------------

        // Formatage
        [Fact]
        public void ToString_NumeroSansFormat_RetourneNumeroFormate() =>
            Assert.Equal("90.01.01-123.95", NumeroNational.Parse("90010112395").ToString());
        [Fact]
        public void ToStringBrute_NumeroSansFormat_RetourneNumeroSansFormat() =>
            Assert.Equal("90010112395", NumeroNational.Parse("90010112395").ToStringBrute());
        [Fact]
        public void ParseToString_Numero_RetourneMemeNumero()
        {
            NumeroNational n = NumeroNational.Parse("90010112395");
            Assert.Equal(n, NumeroNational.Parse(n.ToString()));
        }
        // ------------------------------------------------------------

        // EstCoherentAvec

        [Theory] // (brute, date, sexe, attendu)
        [InlineData("90010112395", "1990-01-01", Sexe.Masculin, true)]      // nominal
        [InlineData("90010112395", "1990-01-01", Sexe.Feminin, false)]      // parité
        [InlineData("90010112395", "1990-01-01", Sexe.NonBinaire, true)]    // pas de contrôle de parité
        [InlineData("90010112395", "1990-01-01", (Sexe)99, false)]          // sexe invalide
        [InlineData("90010112395", "1990-01-02", Sexe.Masculin, false)]     // jour
        [InlineData("90010112395", "1990-02-01", Sexe.Masculin, false)]     // mois
        [InlineData("90010112395", "1991-01-01", Sexe.Masculin, false)]     // année
        [InlineData("99123112442", "1999-12-31", Sexe.Feminin, true)]       // dernier jour de 1999
        [InlineData("00010112476", "2000-01-01", Sexe.Feminin, true)]       // premier jour de 2000
        [InlineData("00010112476", "1900-01-01", Sexe.Feminin, false)]      // siècle
        [InlineData("05031512367", "2005-03-15", Sexe.Masculin, true)]
        [InlineData("05031512367", "1905-03-15", Sexe.Masculin, false)]     // siècle
        [InlineData("05031512338", "1905-03-15", Sexe.Masculin, true)]      // clé 38 = né en 1905
        [InlineData("05031512338", "2005-03-15", Sexe.Masculin, false)]     // siècle
        [InlineData("85462012326", "1985-06-20", Sexe.Masculin, true)]      // BIS +40
        [InlineData("85462012326", "1985-06-20", Sexe.Feminin, false)]      // BIS +40, parité contrôlée
        [InlineData("85462012425", "1985-06-20", Sexe.Feminin, true)]       // BIS +40, SSS pair
        [InlineData("85262012380", "1985-06-20", Sexe.Feminin, true)]       // BIS +20, parité ignorée
        [InlineData("03510512379", "2003-11-05", Sexe.Masculin, true)]      // BIS +40 né en 2003
        [InlineData("00000000097", "2024-05-05", Sexe.Feminin, true)]	    // temporaire
        public void EstCoherentAvec_SelonDateEtSexe_RetourneAttendu(string brute, string date, Sexe sexe, bool attendu)
        {
            DateOnly naissance = DateOnly.Parse(date, CultureInfo.InvariantCulture);
            NumeroNational numero = NumeroNational.Parse(brute);

            Assert.Equal(attendu, numero.EstCoherentAvec(naissance, sexe));
        }

        [Theory]
        [InlineData("90200012364", "1990-01-01", Sexe.Masculin, true)]     // BIS +20
        [InlineData("90260012309", "1990-06-01", Sexe.Masculin, true)]     // BIS +20, mois connu
        [InlineData("91400025692", "1991-01-01", Sexe.Feminin, true)]      // BIS +40
        [InlineData("91400025197", "1991-01-01", Sexe.Feminin, false)]     // BIS +40, parité contrôlée
        // d'autre tests à ajouter
        public void EstCoherentAvec_SelonDateNaissanceInconnuEtSexe_RetourneAttendu(string brute, string dateInconnu, Sexe sexe, bool attendu)
        {
            DateOnly naissance = DateOnly.Parse(dateInconnu, CultureInfo.InvariantCulture);
            NumeroNational numero = NumeroNational.Parse(brute);

            Assert.Equal(attendu, numero.EstCoherentAvec(naissance, sexe));
        }

    }
}
