using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.Infra.Persistance
{
    internal static class WorkflowMapper
    {
        public static Workflow ToDomain(WorkflowRow r) => Workflow.Reconstituer(
            r.IdWorkflow,
            ReferenceDossier.DepuisExistant(r.NumeroDossier, r.Sequence),
            new AuditInfo(r.IdUserExpediteur, r.DateExpedition),
            AssignationDepuis(r),
            r.Message,
            (WorkflowType) r.Type,
            (WorkflowAction) r.Action,
            r.IdDocument,
            r is { IdUserTerminaison: int u, DateTerminaison: DateTime d } ? new AuditInfo(u, d) : null
          );

        private static WorkflowAssignation AssignationDepuis(WorkflowRow r) => r switch
        {
            { IdUserAssigne: int u } => WorkflowAssignation.PourUtilisateur(u),
            { IdGroupeAssigne: int g } => WorkflowAssignation.PourGroupe(g),
            _ => throw new InvalidDataException($"Workflow {r.IdWorkflow} : aucune assignation en base.")
        };
        
    }
}
