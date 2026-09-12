namespace FDL.Core.App
{
    public interface IRepositoryWriteBase<T>
    {
        public bool Add(T entity);

    }
}
