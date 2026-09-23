using FDL.Core.App;

namespace FDL.WF.Tests.Fakes
{
    internal sealed class UtilisateurCourantEnMemoire : IUtilisateurCourant
    {
        private int _id = 1;
        private int[] _idGroupes = [10];

        public int Id => _id;

        public IReadOnlyList<int> IdGroupes => [.. _idGroupes];

        public void ChangerUtilisateur(int id, int[] idGroupes)
        {
            _id = id;
            _idGroupes = idGroupes;
        }
    }
}
