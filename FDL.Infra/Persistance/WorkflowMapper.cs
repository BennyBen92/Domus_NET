using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.Infra.Persistance
{
    internal static class WorkflowMapper
    {
        // Vers le Domain
        public static Workflow ToDomain(WorkflowRow r)
        {
            try
            {
                return Workflow.Reconstituer(
                idWorkflow: r.IdWorkflow,
                refDossier: ReferenceDossier.DepuisExistant(r.NumeroDossier, r.Sequence),
                expediteur: new AuditInfo(r.IdUserExpediteur, r.DateExpedition),
                assignation: AssignationFrom(r),
                message: r.Message,
                type: (WorkflowType)r.Type,
                action: (WorkflowAction)r.Action,
                idDocument: r.IdDocument,
                terminaison: r is { IdUserTerminaison: int u, DateTerminaison: DateTime d } ? new AuditInfo(u, d) : null
              );
            }
            catch (Exception e) when (e is ArgumentException or FormatException)
            {
                throw new InvalidDataException($"Workflow {r.IdWorkflow} : données invalides en base.", e);
            }
        }

        // Crée une Assignation depuis r
        private static WorkflowAssignation AssignationFrom(WorkflowRow r) => r switch
        {
            { IdUserAssigne: int u } => WorkflowAssignation.PourUtilisateur(u),
            { IdGroupeAssigne: int g } => WorkflowAssignation.PourGroupe(g),
            _ => throw new InvalidDataException($"Workflow {r.IdWorkflow} : aucune assignation en base.")
        };
        // ------------------------------------------------------------

        // Vers le Repo
        public static WorkflowRow ToRow(Workflow wf) => new()
        {
            IdWorkflow = wf.IdWorkflow,
            NumeroDossier = wf.ReferenceDossier.NumeroDossier,
            Sequence = wf.ReferenceDossier.Sequence,
            Type = (int)wf.Type,
            Action = (int)wf.Action,
            Message = wf.Message,
            IdUserAssigne = wf.Assignation.IdUser,
            IdGroupeAssigne = wf.Assignation.IdGroup,
            IdUserExpediteur = wf.Expediteur.IdUser,
            DateExpedition = wf.Expediteur.Date,
            IdDocument = wf.IdDocument,
            IdUserTerminaison = wf.Terminaison?.IdUser,
            DateTerminaison = wf.Terminaison?.Date,
            IdUserUpdate = null,
            DateUpdate = null
        };
    }
}
