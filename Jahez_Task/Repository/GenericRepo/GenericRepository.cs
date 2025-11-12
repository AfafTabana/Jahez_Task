using Jahez_Task.Models;
using Microsoft.EntityFrameworkCore;

namespace Jahez_Task.Repository.GenericRepo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        AppDbContext dbContext;
        public GenericRepository(AppDbContext context) {
        
          dbContext = context;
        
        }

        public async Task<IEnumerable<T>> GetAllAsync() {

            IEnumerable<T> Set =  await dbContext.Set<T>().ToListAsync();

            return Set;
        
        
        }

        public async Task<T> GetByIdAsync(int id)
        {
            T Item = await dbContext.Set<T>().FindAsync(id);

            return Item;

        }

        public void Add( T entity )
        {

            dbContext.Set<T>().Add(entity);

        }

        public void Update(T entity) {

            dbContext.Entry(entity).State = EntityState.Modified;
        
        }

        public virtual void Delete(int id) { 
            var entity = dbContext.Set<T>().Find(id);

            if (entity != null)
            {
                dbContext.Set<T>().Remove(entity);
            }
        
        
        }

        public async Task<bool> IsExist(int id) {

            var entity = await GetByIdAsync(id);

            if (entity != null) {
                return true; 
            }else
            {
                return false;
            }
        
        }

    }
}
