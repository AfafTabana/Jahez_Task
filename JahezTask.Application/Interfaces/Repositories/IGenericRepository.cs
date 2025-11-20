namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        void Add(T entity);

        void Update(T entity);

        void Delete(int Id);

        Task<bool> IsExist(int Id);

    }
}
