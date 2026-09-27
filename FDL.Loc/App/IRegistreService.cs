using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Loc.App
{
    public interface IRegistreService
    {
        public int CreerPourLogement(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax,
                                     int[] listeCommunes, bool souhaiteAscenseur, string? commentaire);
        public int CreerPourPMR(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax,
                                     int[] listeCommunes, bool souhaiteAscenseur, string? commentaire);
        public int CreerPourCommerce(ReferenceDossier referenceDossier, int[] listeCommunes, string? commentaire);
        public int CreerPourParking(ReferenceDossier referenceDossier, int[] listeCommunes, string? commentaire);


        public void ChangeListeCommunes(int id, int[] liste);


        public void Terminer(int id);
        public void Radier(int id);


        public Registre? GetByReferenceDossier(ReferenceDossier reference);
    }
}
