using FDL.Core.Domain;

namespace FDL.WF.Domain
{
    public enum EWorkflowType
    {
        Aucun = 0,
        Document = 1,
        Note = 2,
        Tache = 3,
    }
    public enum EWorkflowAction
    {
        Aucune = 0,
        ASigner = 1,
        AValider = 2,
        AVerifier = 3,
        ACorriger = 4,
        ALire = 5
    }

    public class Workflow
    {
        // Props Ids
        public int IdWorkflow { get; } = 0;
        public int IdDocument { get; set; } = 0;
        public ReferenceDossier RefDossier { get; }
        // ---
        // Props Infos
        public string Message { get; set; } = string.Empty;
        public EWorkflowType Type { get; set; } = EWorkflowType.Aucun;
        public EWorkflowAction Action { get; set; } = EWorkflowAction.Aucune;
        // ---
        // Props Assignation
        public int IdUserAssigned { get; set; } = 0;
        public int IdGroupAssigned { get; set; } = 0;
        public int IdUserTermination { get; } = 0;
        public DateTime DateTermination { get; } = DateTime.Now;
        // ---
        // Props Audit
        public int IdUserCreation { get; } = 0;
        public DateTime DateCreation { get; } = DateTime.Now;
        public int IdUserUpdate { get; } = 0;
        public DateTime DateUpdate { get; } = DateTime.Now;

        // Constructeur par défaut
        public Workflow(ReferenceDossier refDossier)
        {
            RefDossier = refDossier;
            
        }

        public Workflow(ReferenceDossier refDossier, EWorkflowType type, EWorkflowAction action, string message, int idDocument)
            : this(refDossier)
        {
            Type = type;
            Action = action;
            Message = message;
            // lequel ?
            IdUserAssigned = 0;
            IdGroupAssigned = 0;

            IdDocument = idDocument;
        }

    }
}
