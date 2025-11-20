using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Persistence.Data;

namespace JahezTask.Persistence.Repositories
{
    public class BookRepository : GenericRepository<Book> , IBookRepository
    {

        private readonly AppDbContext appDbContext; 
        public BookRepository( AppDbContext context ) : base( context ) {
        
            appDbContext = context;
        
        }

        public Book GetBookByTitle(string title)
        {
           Book book = appDbContext.Books.Where(c=> c.Title == title).FirstOrDefault();
           return book;
            
        }



    }
}
