using FDL.Core.App;
using FDL.Core.Domain;

namespace FDL.Core.Infra
{
    public sealed class SessionUtilisateur : IUtilisateurCourant
    {
        private Utilisateur? _utilisateur;
        // ------------------------------------------------------------

        public int Id => _utilisateur?.IdUser ?? throw new InvalidOperationException("L'utilisateur n'est pas connecté.");
        public IReadOnlyList<int> IdGroupes => _utilisateur?.Groupes.Select(g => g.IdGroupe).ToList() ?? throw new InvalidOperationException("L'utilisateur n'est pas connecté.");
        // ------------------------------------------------------------


        public void Connecter(int id, string name)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id, nameof(id));
            _utilisateur = new Utilisateur(id, name);
        }

        public void Deconnecter()
        {
            _utilisateur = null;
        }
    }
}
