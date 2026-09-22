using FDL.Core.App;
using FDL.Core.Domain;

namespace FDL.Infra
{
    public sealed class SessionUtilisateur : IUtilisateurCourant
    {
        private Utilisateur? _utilisateur;
        private IReadOnlyList<int> _idGroupes = [];
        // ------------------------------------------------------------

        private Utilisateur UtilisateurConnecte => _utilisateur ?? throw new InvalidOperationException("L'utilisateur n'est pas connecté.");
        public int Id => UtilisateurConnecte.Id;
        public IReadOnlyList<int> IdGroupes
        {
            get { _ = UtilisateurConnecte; return _idGroupes; }
        }
        // ------------------------------------------------------------


        public void Connecter(Utilisateur utilisateur)
        {
            ArgumentNullException.ThrowIfNull(utilisateur, nameof(utilisateur));
            // Les groupes sont déjà chargés dans utilisateur
            _utilisateur = utilisateur;
            _idGroupes = [.. utilisateur.Groupes.Select(g => g.Id)];
        }

        public void Deconnecter()
        {
            _utilisateur = null;
            _idGroupes = [];
        }
    }
}
