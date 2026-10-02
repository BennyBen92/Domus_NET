using FDL.Core.Domain;

namespace FDL.Core.Tests
{
    public class MenageTests
    {
        private static readonly IReadOnlyList<PersonnePhysique> PersonnePhysiques = [
            PersonnePhysique.Reconstituer(1,"Doe","John",new(1990,1,1),"90010112355",Sexe.Masculin),
            PersonnePhysique.Reconstituer(1,"Doe","Jane",new(1992,12,12),"92121212402",Sexe.Feminin)
            ];
        private static readonly IReadOnlyList<RevenuAnnuel> RevenusAnnuels = [
            new RevenuAnnuel {Montant = 20000, Type = TypeRevenu.Salaire},
            new RevenuAnnuel {Montant = 15000, Type = TypeRevenu.Salaire}
            ];


        [Fact]
        public void CalculerRevenuAnnuelTotal_RetourneLaSomme_35000()
        {
            Menage menage = new([.. PersonnePhysiques], [.. RevenusAnnuels]);

            Assert.Equal(35000, menage.RevenuAnnuelTotal());
        }
    }
}
