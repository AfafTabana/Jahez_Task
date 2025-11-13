using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.DTOs.BookLoan;
using Jahez_Task.Models;

namespace Jahez_Task.Services.BookService
{
    public interface IBookService
    {
        public Task<DisplayBook> GetById(int id);
        public Task<DIsplayBook> GetByIdForAdmin(int id);
        public Task<IEnumerable<DisplayBook>> GetAll();
        public Task<IEnumerable<DIsplayBook>> GetAllBookForAdmin();

        public void AddBook(DIsplayBook book);

        public Task UpdateBook( DIsplayBook book, int BookId);

        public Task<string> DeleteBook( int Id);

        public Task<List<DisplayBook>> GetAvailableBooks();

        public Task<(BookLoan Loan, string Message)> BorrowBook(int UserId , DisplayBook Book);

        public Task<(BookLoan Loan, string Message)> ReturnBook (int UserId, DisplayBook Book);

        public Task<BookLoan> AddBookLoan(int UserId , AddBookLoanDTO BookLoan); 

    }
}
