using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public interface IWorkflowService
    {
        public int CreerPourDocument(ReferenceDossier refDossier, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument);
        public int CreerPourNote(ReferenceDossier refDossier, WorkflowAssignation assignation, string message);
        public int CreerPourTache(ReferenceDossier refDossier, WorkflowAssignation assignation, string message);

        public void Reassigner(int idWorkflow, WorkflowAssignation assignation);
        public void Terminer(int idWorkflow);

        public IReadOnlyList<Workflow> MesWorkflowsEnCours();
        public IReadOnlyList<Workflow> WorkflowsDeMesGroupes();

    }
}
