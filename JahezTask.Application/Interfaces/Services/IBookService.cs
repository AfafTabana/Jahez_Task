using JahezTask.Application.DTOs.Book;
using JahezTask.Application.DTOs.BookLoan;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Interfaces.Services
{
    public interface IBookService
    {
        public Task<DisplayBookForMember> GetById(int id);
        public Task<DisplayBookForAdmin> GetByIdForAdmin(int id);
        public Task<IEnumerable<DisplayBookForMember>> GetAll();
        public Task<IEnumerable<DisplayBookForAdmin>> GetAllBookForAdmin();

        public void AddBook(DisplayBookForAdmin book);

        public Task UpdateBook( DisplayBookForAdmin book, int BookId);

        public Task<string> DeleteBook( int Id);

        public Task<List<DisplayBookForMember>> GetAvailableBooks();

        public Task<(BookLoan Loan, string Message)> BorrowBook(int UserId , DisplayBookForMember Book);

        public Task<(BookLoan Loan, string Message)> ReturnBook (int UserId, DisplayBookForMember Book);

        public Task<BookLoan> AddBookLoan(int UserId , AddBookLoanDTO BookLoan); 

    }
}
