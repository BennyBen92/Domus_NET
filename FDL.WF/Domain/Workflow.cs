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
        public int IdWorkflow { get; private set; } = 0;
        public int IdDocument { get; private set; } = 0;
        public ReferenceDossier RefDossier { get; }
        // ---
        // Props Infos
        public string Message { get; private set; } = string.Empty;
        public EWorkflowType Type { get; private set; } = EWorkflowType.Aucun;
        public EWorkflowAction Action { get; private set; } = EWorkflowAction.Aucune;
        // ---
        // Props Assignation
        public WorkflowAssignation Assignation { get; private set; }
        private int IdUserAssigned { get; set; } = 0;
        private int IdGroupAssigned { get; set; } = 0;

        public int IdUserTermination { get; private set; } = 0;
        public DateTime DateTermination { get; private set; } = DateTime.MinValue;
        // ---
        // Props Audit
        public int IdUserCreation { get; } = 0;
        public DateTime DateCreation { get; } = DateTime.Now;
        public int IdUserUpdate { get; private set; } = 0;
        public DateTime DateUpdate { get; private set; } = DateTime.Now;

        // Constructeur par défaut
        private Workflow(ReferenceDossier refDossier, EWorkflowType type, EWorkflowAction action, WorkflowAssignation assignation, string message)
        {
            RefDossier = refDossier;
            Type = type;
            Action = action;
            Message = message;
            Assignation = assignation;
            IdUserAssigned = assignation.IdUser ?? 0;
            IdGroupAssigned = assignation.IdGroup ?? 0;
        }
        private Workflow(ReferenceDossier refDossier, EWorkflowType type, EWorkflowAction action, WorkflowAssignation assignation, string message, int idDocument)
            : this(refDossier, type, action, assignation, message)
        {
            IdDocument = idDocument;
        }

        // Méthodes statiques pour créer des instances de Workflow
        public static Workflow PourNote(ReferenceDossier refDossier, EWorkflowAction action, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, EWorkflowType.Note, action, assignation, message);
        }
        public static Workflow PourDocument(ReferenceDossier refDossier, EWorkflowAction action, WorkflowAssignation assignation, string message, int idDocument)
        {
            return new Workflow(refDossier, EWorkflowType.Document, action, assignation, message, idDocument);
        }
        public static Workflow PourTache(ReferenceDossier refDossier, EWorkflowAction action, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, EWorkflowType.Tache, action, assignation, message);
        }

        /**
         * Méthode pour réassigner le workflow à une nouvelle assignation.
         * Cette méthode met à jour l'assignation ainsi que les identifiants d'utilisateur et de groupe assignés.
         */
        public void Réassigner(WorkflowAssignation nouvelleAssignation)
        {
            Assignation = nouvelleAssignation;
            IdUserAssigned = nouvelleAssignation.IdUser ?? 0;
            IdGroupAssigned = nouvelleAssignation.IdGroup ?? 0;
        }
        /**
         * Méthode pour terminer le workflow.
         * Cette méthode met à jour l'identifiant de l'utilisateur qui termine le workflow ainsi que la date de terminaison.
         * @param idUser L'identifiant de l'utilisateur qui termine le workflow.
         * @throws ArgumentOutOfRangeException Si l'identifiant de l'utilisateur est inférieur ou égal à zéro.
         */
        public void Terminer(int idUser)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(idUser, 0, nameof(idUser));
            IdUserTermination = idUser;
            DateTermination = DateTime.Now;
        }
    }
}
