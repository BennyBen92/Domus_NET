using FDL.Core.App;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    internal interface IWorkflowRepository : IRepositoryReadBase<Workflow>, IRepositoryWriteBase<Workflow>
    {
        Workflow GetByUserAssigned(int idUser);
        Workflow GetByGroupAssigned(int idGroup);

    }
}
