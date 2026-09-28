using FDL.Core.App;
using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Loc.App
{
    public sealed class RegistreService(
        IRegistreRepository registreRepository,
        IUtilisateurCourant utilisateurCourant,
        TimeProvider timeProvider) : IRegistreService
    {
        private readonly IRegistreRepository _registreRepository = registreRepository;
        private readonly IUtilisateurCourant _utilisateurCourant = utilisateurCourant;
        private readonly TimeProvider _timeProvider = timeProvider;
        // ------------------------------------------------------------

        // Utilisateur qui agit
        private AuditInfo FaitPar() => new(_utilisateurCourant.Id, _timeProvider.GetUtcNow().UtcDateTime);

        // ------------------------------------------------------------

        public int CreerPourLogement(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax, int[] listeCommunes, bool souhaiteAscenseur, string? commentaire)
        {
            var registre = Registre.PourLogement(referenceDossier, nbChambresMin, nbChambresMax, listeCommunes,
                                                 souhaiteAscenseur, FaitPar(), commentaire);
            return _registreRepository.Add(registre);
        }
        public int CreerPourPMR(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax, int[] listeCommunes, bool souhaiteAscenseur, string? commentaire)
        {
            var registre = Registre.PourPMR(referenceDossier, nbChambresMin, nbChambresMax, listeCommunes,
                                            souhaiteAscenseur, FaitPar(), commentaire);
            return _registreRepository.Add(registre);
        }

        public int CreerPourCommerce(ReferenceDossier referenceDossier, int[] listeCommunes, string? commentaire)
        {
            var registre = Registre.PourCommerce(referenceDossier, listeCommunes, FaitPar(), commentaire);
            return _registreRepository.Add(registre);
        }

        public int CreerPourParking(ReferenceDossier referenceDossier, int[] listeCommunes, string? commentaire)
        {
            var registre = Registre.PourParking(referenceDossier, listeCommunes, FaitPar(), commentaire);
            return _registreRepository.Add(registre);
        }


        public void ChangeListeCommunes(int id, int[] liste)
        {
            Registre? registre = _registreRepository.GetById(id)
                ?? throw new InvalidOperationException($"Le registre {id} n'existe pas.");

            registre!.ChangeListeCommunes(liste);
            _registreRepository.Update(registre, FaitPar());
        }



        public void Radier(int id)
        {
            Registre? registre = _registreRepository.GetById(id)
                ?? throw new InvalidOperationException($"Le registre {id} n'existe pas.");

            AuditInfo faitPar = FaitPar();
            registre.Radier(faitPar.Date);
            _registreRepository.Update(registre, faitPar);
        }
        public void Terminer(int id)
        {
            Registre registre = _registreRepository.GetById(id)
                ?? throw new InvalidOperationException($"Le registre {id} n'existe pas.");

            AuditInfo faitPar = FaitPar();
            registre!.Terminer(faitPar.Date);
            _registreRepository.Update(registre, faitPar);
        }

        // ------------------------------------------------------------


        public Registre? GetByReferenceDossier(ReferenceDossier reference)
        {
            ArgumentNullException.ThrowIfNull(reference);
            return _registreRepository.GetByReferenceDossier(reference);
        }
    }
}
