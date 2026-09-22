using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public interface IWorkflowService
    {
        public int CreerWorkflowDocument(ReferenceDossier refDossier, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument);

        public int CreerWorkflowNote(ReferenceDossier refDossier, WorkflowAssignation assignation, string message);

        public int CreerWorkflowTache(ReferenceDossier refDossier, WorkflowAssignation assignation, string message);

        public void Terminer(int idWorkflow);

        public IReadOnlyList<Workflow> MesWorkflowsEnCours();

    }
}
