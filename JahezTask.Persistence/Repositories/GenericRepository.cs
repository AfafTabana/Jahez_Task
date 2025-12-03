using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace JahezTask.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        AppDbContext dbContext;
        protected readonly DbSet<T> _dbSet;
        private IDbContextTransaction _transaction;
        public GenericRepository(AppDbContext context) {
        
          dbContext = context;
          _dbSet = context.Set<T>();

        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) {

            IEnumerable<T> Set =  await dbContext.Set<T>().ToListAsync(cancellationToken);

            return Set;
        
        
        }


        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id , CancellationToken cancellationToken = default)
        {
            T Item = await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);

            return Item;

        }

        public void Add( T entity )
        {

            dbContext.Set<T>().Add(entity);

        }

        public void Update(T entity) {

            var key = dbContext.Entry(entity).Property("Id").CurrentValue;
            var existing = dbContext.Set<T>().Find(key);

            if (existing == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {key} not found.");
            }

            dbContext.Entry(existing).CurrentValues.SetValues(entity);

        }

        public void Delete(int id) { 
            var entity = dbContext.Set<T>().Find(id);

            if (entity != null)
            {
                dbContext.Set<T>().Remove(entity);

            }

            

        }

        public async Task<bool> IsExist(int id , CancellationToken cancellationToken = default) {

            
            var entity = await GetByIdAsync(id , cancellationToken);

            if (entity != null) {
                return true; 
            }
            else
            {
                return false;
            }
        
        }


        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to commit.");
            }

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to rollback.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }


    }
}
