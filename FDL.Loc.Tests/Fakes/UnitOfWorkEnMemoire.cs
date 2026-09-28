using FDL.Core.App;

namespace FDL.Loc.Tests.Fakes
{
    /// <summary>
    /// Unit of Work pour les tests : exécute l'action sans vraie transaction,
    /// mais enregistre ce qui s'est passé (validée ou annulée).
    /// Le rollback réel se vérifie par un test d'intégration contre PostgreSQL.
    /// </summary>
    internal sealed class UnitOfWorkEnMemoire : IUnitOfWork
    {
        public int NbExecutions { get; private set; }
        public bool EstValidee { get; private set; }
        public bool EstAnnulee { get; private set; }

        public T Execute<T>(Func<T> action)
        {
            NbExecutions++;
            try
            {
                T resultat = action();
                EstValidee = true;
                return resultat;
            }
            catch
            {
                EstAnnulee = true;
                throw;
            }
        }
    }
}
