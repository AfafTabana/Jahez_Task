using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;

namespace Jahez_Task.Repository.BookRepo
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
