using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace JahezTask.Persistence.Repositories
{
    public class BookRepository : GenericRepository<Book> , IBookRepository
    {

        private readonly AppDbContext appDbContext; 
        public BookRepository( AppDbContext context ) : base( context ) {
        
            appDbContext = context;
        
        }

        public async Task<Book> GetBookByTitle(string title , CancellationToken cancellationToken = default)
        {
           Book book = await appDbContext.Books.AsNoTracking().Where(c=> c.Title == title).FirstOrDefaultAsync(cancellationToken);
           return book;
            
        }



    }
}
