using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.DTOs.BookLoan;

namespace Jahez_Task.Services.BookService
{
    public interface IBookService
    {
        public Task<DisplayBook> GetById(int id);
        public Task<DIsplayBook> GetByIdForAdmin(int id);
        public Task<IEnumerable<DisplayBook>> GetAll();
        public Task<IEnumerable<DIsplayBook>> GetAllBookForAdmin();

        public void AddBook(DIsplayBook book);

        public void UpdateBook( DIsplayBook book);

        public Task DeleteBook( int Id);

        public Task<List<DisplayBook>> GetAvailableBooks();

        public void BorrowBook(int UserId , DisplayBook Book);

        public Task ReturnBook (int UserId, DisplayBook Book);

        public void AddBookLoan(int UserId , AddBookLoanDTO BookLoan); 

    }
}
