using FDL.Core.Domain;

namespace FDL.WF.Domain
{
    public enum WorkflowType
    {
        Inconnu = 0,
        Document = 1,
        Note = 2,
        Tache = 3,
    }
    public enum WorkflowAction
    {
        Inconnu = 0,
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
        public WorkflowType Type { get; private set; } = WorkflowType.Inconnu;
        public WorkflowAction Action { get; private set; } = WorkflowAction.Inconnu;
        // ---
        // Props Assignation
        public WorkflowAssignation Assignation { get; private set; }
        public WorkflowTerminaison? Terminaison;

        // ---
        // Props Audit
        public int IdUserCreation { get; } = 0;
        public DateTime DateCreation { get; } = DateTime.Now;
        public int IdUserUpdate { get; private set; } = 0;
        public DateTime DateUpdate { get; private set; } = DateTime.Now;

        // Constructeur par défaut
        private Workflow(ReferenceDossier refDossier, WorkflowType type, WorkflowAction action, WorkflowAssignation assignation, string message)
        {
            RefDossier = refDossier;
            Type = type;
            Action = action;
            Message = message;
            Assignation = assignation;
        }
        private Workflow(ReferenceDossier refDossier, WorkflowType type, WorkflowAction action, WorkflowAssignation assignation, string message, int idDocument)
            : this(refDossier, type, action, assignation, message)
        {
            IdDocument = idDocument;
        }

        public bool EstTermine => Terminaison is not null;

        // Méthodes statiques pour créer des instances de Workflow
        public static Workflow PourNote(ReferenceDossier refDossier, WorkflowAction action, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, WorkflowType.Note, action, assignation, message);
        }
        public static Workflow PourDocument(ReferenceDossier refDossier, WorkflowAction action, WorkflowAssignation assignation, string message, int idDocument)
        {
            return new Workflow(refDossier, WorkflowType.Document, action, assignation, message, idDocument);
        }
        public static Workflow PourTache(ReferenceDossier refDossier, WorkflowAction action, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, WorkflowType.Tache, action, assignation, message);
        }

        /// <summary>
        /// Réassigne le workflow à une nouvelle assignation.
        /// </summary>
        /// <param name="nouvelleAssignation">La nouvelle assignation.</param>
        public void Reassigner(WorkflowAssignation nouvelleAssignation)
        {
            Assignation = nouvelleAssignation;
        }
        /// <summary>
        /// Marque le workflow comme terminé en enregistrant l'identifiant de l'utilisateur qui a terminé le workflow et la date de terminaison.
        /// </summary>
        /// <param name="idUser"></param>
        public void Terminer(int idUser)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUser, nameof(idUser));
            if (EstTermine)
            {
                throw new InvalidOperationException("Le workflow est déjà terminé.");
            }
            Terminaison = new WorkflowTerminaison(idUser, DateTime.Now);
        }
    }
}
