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


    public class Workflow
    {
        // Props Ids
        public int IdWorkflow { get; private set; }
        public int IdDocument { get; }
        public ReferenceDossier RefDossier { get; }

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


        // Constructeur par défaut
        private Workflow(ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action)
        {
            RefDossier = refDossier;
            Expediteur = expediteur;
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
        private Workflow(int idWorkflow, ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument, AuditInfo terminaison)
            : this(refDossier, expediteur, assignation, message, type, action)
        {
            Terminaison = terminaison;
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

        public static Workflow Reconstituer(int idWorkflow, ReferenceDossier refDossier, AuditInfo expediteur, WorkflowAssignation assignation, string message, WorkflowType type, WorkflowAction action, int idDocument, AuditInfo? terminaison)
        {
            return new Workflow(idWorkflow, refDossier, expediteur, assignation, message, type, action, idDocument, terminaison);
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
