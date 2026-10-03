namespace FDL.Infra.Persistance
{
    internal sealed class PersonnePhysiqueRow
    {
        public int IdPersonnePhysique { get; set; }
        public string Nom { get; set; } = "";
        public string? Prenom { get; set; }
        public DateOnly DateNaissance { get; set; }      // colonne date, sans conversion
        public string NumeroNational { get; set; } = ""; // 11 chiffres, sans séparateurs
        public int Sexe { get; set; }
    }
}
