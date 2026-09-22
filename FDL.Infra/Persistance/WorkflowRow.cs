namespace FDL.Infra.Persistance
{
    internal sealed class WorkflowRow
    {
        public int IdWorkflow { get; set; }
        public int NumeroDossier { get; set; }
        public int Sequence { get; set; }
        public int Type { get; set; }
        public int Action { get; set; }
        public string Message { get; set; } = "";
        public int? IdUserAssigne { get; set; }
        public int? IdGroupeAssigne { get; set; }
        // Expéditeur est le créateur
        public int IdUserExpediteur { get; set; }
        public DateTime DateExpedition { get; set; }
        public int? IdUserTerminaison { get; set; }
        public DateTime? DateTerminaison { get; set; }
        public int IdDocument { get; set; }
        
        // Audit Update en nullable, car il n'est géré qu'en base
        public int? IdUserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
