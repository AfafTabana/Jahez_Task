namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetQueryable();
        Task<T> GetByIdAsync(int id);

        void Add(T entity);

        void Update(T entity);

        void Delete(int id);

        Task<bool> IsExist(int id);

    }
}
