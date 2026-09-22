using FDL.Core.App;

namespace FDL.Core.Infra
{
    public sealed class SessionUtilisateur : IUtilisateurCourant
    {
        private int? _id;
        public int Id => _id ?? throw new InvalidOperationException("L'utilisateur n'est pas connecté.");

        public void Connecter(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id, nameof(id));
            _id = id;
        }

        public void Deconnecter()
        {
            _id = null;
        }
    }
}
