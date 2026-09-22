using FDL.Core.Domain;

namespace FDL.WF.Domain
{
    public enum WorkflowType
    {
        Document = 1,
        Note = 2,
        Tache = 3,
    }
    public enum WorkflowAction
    {
        ASigner = 1,
        AValider = 2,
        AVerifier = 3,
        ACorriger = 4,
        ALire = 5
    }
    // ------------------------------------------------------------
    // ------------------------------------------------------------


    public sealed class Workflow
    {
        // Props Ids
        public int IdWorkflow { get; }
        public int IdDocument { get; }
        public ReferenceDossier ReferenceDossier { get; }

        // Props Infos
        public string Message { get; }
        public WorkflowType Type { get; }
        public WorkflowAction Action { get; }

        // A qui est assigné le wf
        public WorkflowAssignation Assignation { get; private set; }

        // Qui a validé, terminé le wf
        public AuditInfo? Terminaison { get; private set; }

        // Qui a créé et envoyé le wf
        public AuditInfo Expediteur { get; }
        // ------------------------------------------------------------


        // Constructeur
        private Workflow(int idWorkflow, ReferenceDossier referenceDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument, AuditInfo? terminaison)
        {
            IdWorkflow = idWorkflow;
            ReferenceDossier = referenceDossier;
            Expediteur = expediteur;
            Assignation = assignation;
            Message = message;
            Type = type;
            Action = action;
            IdDocument = idDocument;
            Terminaison = terminaison;
        }
        // ------------------------------------------------------------


        public bool EstTermine => Terminaison is not null;
        // ------------------------------------------------------------


        // Fabrique pour créer des instances de Workflow
        /// <summary>
        /// Crée un workflow de type Note avec l'action ALire.
        /// </summary>
        public static Workflow PourNote(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(
                idWorkflow: 0,
                referenceDossier: refDossier,
                expediteur: expediteur,
                assignation: assignation,
                message: message,
                type: WorkflowType.Note,
                action: WorkflowAction.ALire,
                idDocument: 0,
                terminaison: null);
        }
        /// <summary>
        /// Crée un workflow de type Document avec l'action spécifiée et l'identifiant du document.
        /// </summary>
        public static Workflow PourDocument(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idDocument, nameof(idDocument));
            return new(
                idWorkflow: 0,
                referenceDossier: refDossier,
                expediteur: expediteur,
                assignation: assignation,
                message: message,
                type: WorkflowType.Document,
                action: action,
                idDocument: idDocument,
                terminaison: null);
        }

        /// <summary>
        /// Crée un workflow de type Tache avec l'action AValider.
        /// </summary>
        public static Workflow PourTache(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(
                idWorkflow: 0,
                referenceDossier: refDossier,
                expediteur: expediteur,
                assignation: assignation,
                message: message,
                type: WorkflowType.Tache,
                action: WorkflowAction.AValider,
                idDocument: 0,
                terminaison: null);
        }
        // ------------------------------------------------------------

        public static Workflow Reconstituer(int idWorkflow, ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument, AuditInfo? terminaison)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idWorkflow, nameof(idWorkflow));
            if (!Enum.IsDefined(type))
                throw new InvalidDataException($"Workflow {idWorkflow} : type invalide ({(int)type}).");
            if (!Enum.IsDefined(action))
                throw new InvalidDataException($"Workflow {idWorkflow} : action invalide ({(int)action}).");
            if (type is WorkflowType.Document && idDocument <= 0)
                throw new InvalidDataException($"Workflow {idWorkflow} : document manquant.");
            return new Workflow(idWorkflow, refDossier, expediteur, assignation, message, type, action, idDocument, terminaison);
        }

        // ------------------------------------------------------------


        /// <summary>
        /// Réassigne le workflow à une nouvelle assignation.
        /// </summary>
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
        public void Terminer(AuditInfo auditInfo)
        {
            if (EstTermine)
            {
                throw new InvalidOperationException("Le workflow est déjà terminé.");
            }
            Terminaison = auditInfo;
        }
    }
}
