namespace FDL.Core.App
{
    public interface IWriteRepository<T>
    {
        public int Add(T entity);
        public int Update(T entity);

    }
}
