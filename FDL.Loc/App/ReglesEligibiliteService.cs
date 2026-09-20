using FDL.Core.Domain;

namespace FDL.Loc.App
{
    public class ReglesEligibiliteService : IReglesEligibiliteService
    {
        private readonly decimal _plafondParPersonne = 15000m;

        /// <summary>
        /// Évalue l'éligibilité d'un ménage en fonction de son revenu total et du plafond par personne.
        /// </summary>
        /// <param name="menage"></param>
        public EligibiliteResult Evaluer(Menage menage)
        {
            var plafond = _plafondParPersonne * menage.NombrePersonnes;
            var revenu = menage.RevenuTotal();

            return revenu <= plafond ?
                EligibiliteResult.Eligible($"Revenu {revenu:C} <= plafond {plafond:C}") :
                EligibiliteResult.NonEligible($"Revenu {revenu:C} > plafond {plafond:C}");
        }
    }
}
