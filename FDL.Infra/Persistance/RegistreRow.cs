namespace FDL.Infra.Persistance
{
    internal sealed class RegistreRow
    {
        public int IdRegistre { get; set; }
        public int ReferenceDossier { get; set; }
        public int IdUserCreation { get; set; }
        public DateTime DateCreation { get; set; }
        public int Type { get; set; }
        public int Statut { get; set; }
        public DateTime DateStatut { get; set; }
        public string? Commentaire { get; set; }
        public int NbChambresMin { get; set; }
        public int NbChambresMax { get; set; }
        public int[] ListeCommunes { get; set; } = [];
        public bool SouhaiteAscenseur { get; set; }

        // Audit Update en nullable, car il n'est géré qu'en base
        public int? IdUserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
