namespace FDL.Core.App
{
    public interface IWriteRepository<T>
    {
        public bool Add(T entity);

    }
}
