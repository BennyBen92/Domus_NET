using FDL.Core.Domain;

namespace FDL.Loc.App
{
    public interface IInscriptionService
    {
        public (int IdRegistre, int IdWorkflow) InscriptionPourLogement(ReferenceDossier referenceDossier,
                                                                        int nbChambresMin, int nbChambresMax,
                                                                        int[] listeCommunes, bool souhaiteAscenseur,
                                                                        string? messageDuCandidat);
    }
}
