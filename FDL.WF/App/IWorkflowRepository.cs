using FDL.Core.App;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public interface IWorkflowRepository : IReadRepository<Workflow>, IWriteRepository<Workflow>
    {
        IReadOnlyList<Workflow> GetByUserAssigned(int idUser);
        IReadOnlyList<Workflow> GetByGroupAssigned(int idGroup);

        // Pour récupérer par exemple les wf "terminés" assignés à un utilisateur,
        // Comment faire ? On utilise GetByUserAssigned et on filtre sur IdUserTermination != 0 ?
        // Ou on ajoute une méthode GetByUserAssignedTerminated(int idUser) ?
    }
}
