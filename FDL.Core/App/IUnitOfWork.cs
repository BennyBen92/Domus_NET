namespace FDL.Core.App
{
    public interface IUnitOfWork
    {
        public T Execute<T>(Func<T> action);
    }
}
