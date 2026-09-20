namespace FDL.Core.App
{
    public interface IReadRepository<T>
    {
        public T? GetById(int id);
        public IEnumerable<T> GetAll();

    }
}
