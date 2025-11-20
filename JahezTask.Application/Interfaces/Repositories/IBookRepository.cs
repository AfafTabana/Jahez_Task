using JahezTask.Application.DTOs.Book;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {

        public Book GetBookByTitle (string title);
        
    }
}
