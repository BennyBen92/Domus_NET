using FDL.Core.App;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public interface IWorkflowRepository : IReadRepository<Workflow>, IWriteRepository<Workflow>
    {
        IReadOnlyList<Workflow> Search(WorkflowFiltre filtre);
    }
}
