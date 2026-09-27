using FDL.Core.Domain;

namespace FDL.Infra.Persistance
{
    internal static class ReferenceDossierCodec
    {
        public static int ToInt(ReferenceDossier r) => r.NumeroDossier * 100 + r.Sequence;

        public static ReferenceDossier FromInt(int num) => 
            ReferenceDossier.DepuisExistant(num / 100, num % 100);
    }
}
