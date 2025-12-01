namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        IQueryable<T> GetQueryable();
        Task<T> GetByIdAsync(int id , CancellationToken cancellationToken = default);

        void Add(T entity);

        void Update(T entity);

        void Delete(int id);

        Task<bool> IsExist(int id , CancellationToken cancellationToken = default);

    }
}
