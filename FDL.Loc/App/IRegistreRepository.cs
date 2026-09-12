using FDL.Core.App;
using FDL.Loc.Domain;

namespace FDL.Loc.App
{
    internal interface IRegistreRepository : IRepositoryReadBase<Registre>, IRepositoryWriteBase<Registre>
    {
        Registre GetByContrat(int idContrat);
        
    }
}
