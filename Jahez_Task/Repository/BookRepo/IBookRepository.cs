using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;

namespace Jahez_Task.Repository.BookRepo
{
    public interface IBookRepository : IGenericRepository<Book>
    {

        public Book GetBookByTitle (string title);
        
    }
}
