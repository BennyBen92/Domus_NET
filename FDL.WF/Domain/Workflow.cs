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
    // ------------------------------------------------------------
    // ------------------------------------------------------------


    public class Workflow
    {
        // Props Ids
        public int IdWorkflow { get; private set; } = 0;
        public int IdDocument { get; private set; } = 0;
        public ReferenceDossier RefDossier { get; }
        
        // Props Infos
        public string Message { get; private set; } = string.Empty;
        public WorkflowType Type { get; private set; } = WorkflowType.Inconnu;
        public WorkflowAction Action { get; private set; } = WorkflowAction.Inconnu;
        
        // Props Assignation
        public WorkflowAssignation Assignation { get; private set; }
        
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
        }
        private Workflow(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument)
            : this(refDossier, expediteur, assignation, message, type, action)
        {
            IdDocument = idDocument;
        }
        // ------------------------------------------------------------


        public bool EstTermine => Terminaison is not null;
        // ------------------------------------------------------------


        // Fabrique pour créer des instances de Workflow
        /// <summary>
        /// Crée un workflow de type Note avec l'action ALire.
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="expediteur"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Workflow PourNote(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Note, WorkflowAction.ALire);
        }

        /// <summary>
        /// Crée un workflow de type Document avec l'action spécifiée et l'identifiant du document.
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="expediteur"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static Workflow PourDocument(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idDocument, nameof(idDocument));
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Document, action, idDocument);
        }

        /// <summary>
        /// Crée un workflow de type Tache avec l'action AValider.
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="expediteur"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Workflow PourTache(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message)
        {
            return new Workflow(refDossier, expediteur, assignation, message, WorkflowType.Tache, WorkflowAction.AValider);
        }
        // ------------------------------------------------------------


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
        /// <param name="auditInfo">Les informations d'audit.</param>
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
