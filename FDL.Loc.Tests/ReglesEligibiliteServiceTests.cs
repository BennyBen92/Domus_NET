using FDL.Core.Domain;
using FDL.Loc.App;

namespace FDL.Loc.Tests
{
    public class ReglesEligibiliteServiceTests
    {
        private static readonly ReglesEligibiliteService _service = new();
        private static readonly IReadOnlyList<PersonnePhysique> PersonnePhysiques = [
            PersonnePhysique.Reconstituer(1,"Doe","John",new(1990,1,1),"90010112355",Sexe.Masculin),
            PersonnePhysique.Reconstituer(2,"Doe","Jane",new(1992,12,12),"92121212402",Sexe.Feminin)
            ];


        [Fact]
        public void Menage_SousLePlafond_EstEligible()
        {
            var menage = new Menage(
                [.. PersonnePhysiques],
                [new RevenuAnnuel() { Montant = 20000, Type = TypeRevenu.Salaire }]
                );

            var result = _service.Evaluer(menage);

            Assert.True(result.EstEligible, "Le ménage devrait être éligible.");
        }
        [Fact]
        public void Menage_AuPlafond_EstEligible()
        {
            var menage = new Menage(
                [.. PersonnePhysiques],
                [new RevenuAnnuel() { Montant = 30000, Type = TypeRevenu.Salaire }]
                );

            var result = _service.Evaluer(menage);

            Assert.True(result.EstEligible, "Le ménage devrait être éligible.");
        }
        [Fact]
        public void Menage_SuperieurAuPlafond_NonEligible()
        {
            var menage = new Menage(
                [.. PersonnePhysiques],
                [new RevenuAnnuel() { Montant = 40000, Type = TypeRevenu.Salaire }]
                );

            var result = _service.Evaluer(menage);

            Assert.False(result.EstEligible, "Le ménage devrait être non éligible.");
        }
    }
}
