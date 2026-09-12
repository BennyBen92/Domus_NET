using FDL.Core.Domain;

namespace FDL.Loc.App
{
    public interface IReglesEligibiliteService
    {
        EligibiliteResult Evaluer(Menage menage);
    }
}
