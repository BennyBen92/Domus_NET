using FDL.Core.App;

namespace FDL.Infra.Persistance
{
    internal sealed class UnitOfWork(DomusDbContext db) : IUnitOfWork
    {
        public T Execute<T>(Func<T> action)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
                T resultat = action();
                transaction.Commit();
                return resultat;
            }
            catch
            {
                transaction.Rollback(); // Déjà appelé par Dispose gràce au 'using'
                throw;
            }
        }
    }
}
