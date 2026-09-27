using FDL.Core.App;
using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Loc.App
{
    public interface IRegistreRepository : IReadRepository<Registre>, IWriteRepository<Registre>
    {
        Registre? GetByReferenceDossier(ReferenceDossier reference);
        
    }
}
