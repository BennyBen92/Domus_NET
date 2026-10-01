namespace FDL.Core.Domain
{
    public sealed class Menage
    {
        public IReadOnlyList<PersonnePhysique> Personnes { get; private set; }
        public IReadOnlyList<RevenuAnnuel> RevenusAnnuel { get; private set; }

        public Menage(List<PersonnePhysique> personnes, List<RevenuAnnuel> revenusAnnuel)
        {
            Personnes = [.. personnes];
            RevenusAnnuel = [.. revenusAnnuel];
        }



        public int NombrePersonnes => Personnes.Count;

        public decimal RevenuAnnuelTotal() => RevenusAnnuel.Sum(revenu => revenu.Montant);
        
    }
}
