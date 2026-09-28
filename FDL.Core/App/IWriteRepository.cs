using FDL.Core.Domain;

namespace FDL.Core.App
{
    public interface IWriteRepository<T>
    {
        public int Add(T entity);
        public void Update(T entity, AuditInfo modifiePar);
    }
}
