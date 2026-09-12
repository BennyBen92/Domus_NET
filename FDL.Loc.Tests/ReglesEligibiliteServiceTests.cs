using FDL.Core.Domain;
using FDL.Loc.App;

namespace FDL.Loc.Tests
{
    public class ReglesEligibiliteServiceTests
    {
        [Fact]
        public void Menage_SousLePlafond_EstEligible()
        {
            var service = new ReglesEligibiliteService();
            var menage = new Menage { NombrePersonnes = 2 };
            menage.Revenus.Add(new RevenuAnnuel { Montant = 20000, Type = TypeRevenu.Salaire });

            var result = service.Evaluer(menage);

            Assert.True(result.EstEligible);
        }
        [Fact]
        public void Menage_AuPlafond_EstEligible()
        {
            var service = new ReglesEligibiliteService();
            var menages = new Menage {
                NombrePersonnes = 2
            };
            menages.Revenus.Add(new RevenuAnnuel { Montant = 30000, Type = TypeRevenu.Salaire });

            var result = service.Evaluer(menages);

            Assert.True(result.EstEligible);
        }
        [Fact]
        public void Menage_SuperieurAuPlafond_NonEligible()
        {
            var service = new ReglesEligibiliteService();
            var menage = new Menage { NombrePersonnes = 2 };
            menage.Revenus.Add(new RevenuAnnuel { Montant = 40000, Type = TypeRevenu.Salaire });

            var result = service.Evaluer(menage);

            Assert.False(result.EstEligible);
        }
    }
}
