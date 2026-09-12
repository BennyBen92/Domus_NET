namespace FDL.Core.Domain
{
    public enum TypeRevenu
    {
        Salaire,
        Allocation,
        Pension,
        Independant,
        Autre
    }
    public class RevenuAnnuel
    {
        public decimal Montant { get; set; }
        public TypeRevenu Type { get; set; }
        public RevenuAnnuel()
        {
            Montant = 0;
            Type = TypeRevenu.Autre;
        }
    }
}
