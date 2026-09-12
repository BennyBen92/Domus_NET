using FDL.Core.App;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public interface IWorkflowRepository : IReadRepository<Workflow>, IWriteRepository<Workflow>
    {
        Workflow GetByUserAssigned(int idUser);
        Workflow GetByGroupAssigned(int idGroup);

    }
}
