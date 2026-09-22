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
        // ---
        // Props Terminaison
        public AuditInfo? Terminaison { get; private set; }

        public AuditInfo Creation { get; private set; }
        // ------------------------------------------------------------


        // Constructeur par défaut
        private Workflow(ReferenceDossier refDossier, AuditInfo expediteur , WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action)
        {
            RefDossier = refDossier;
            Creation = expediteur;
            Assignation = assignation;
            Message = message;
            Type = type;
            Action = action;
            Message = message;
            Assignation = assignation;
        }
        private Workflow(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument)
            : this(refDossier, expediteur, assignation, message, type, action)
        {
            IdDocument = idDocument;
        }
        // ------------------------------------------------------------

        public bool EstTermine => Terminaison is not null;

        // Méthodes statiques pour créer des instances de Workflow
        public static Workflow PourNote(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
        public static Workflow PourNote(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Note, WorkflowAction.ALire);
        }
        public static Workflow PourDocument(ReferenceDossier refDossier, WorkflowAction action, WorkflowAssignation assignation, string message, int idDocument)
        public static Workflow PourDocument(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idDocument, nameof(idDocument));
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Document, action, idDocument);
        }
        public static Workflow PourTache(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
        public static Workflow PourTache(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Tache, WorkflowAction.AValider);
        }

        /// <summary>
        /// Réassigne le workflow à une nouvelle assignation.
        /// </summary>
        /// <param name="nouvelleAssignation">La nouvelle assignation.</param>
        public void Reassigner(WorkflowAssignation nouvelleAssignation)
        {
            if (EstTermine)
            {
                throw new InvalidOperationException("Le workflow est terminé et ne peut pas être réassigné.");
            }
            Assignation = nouvelleAssignation;
        }
        /// <summary>
        /// Marque le workflow comme terminé en enregistrant l'identifiant de l'utilisateur qui a terminé le workflow et la date de terminaison.
        /// </summary>
        /// <param name="idUser"></param>
        public void Terminer(AuditInfo auditInfo)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUser, nameof(idUser));
            if (EstTermine)
            {
                throw new InvalidOperationException("Le workflow est déjà terminé.");
            }
            Terminaison = auditInfo;
        }
    }
}
