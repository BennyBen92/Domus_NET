using FDL.Core.App;
using FDL.Loc.Domain;

namespace FDL.Loc.App
{
    public interface IRegistreRepository : IReadRepository<Registre>, IWriteRepository<Registre>
    {
        public Registre GetByContrat(int idContrat);
        
    }
}
