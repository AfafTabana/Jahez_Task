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

        public Task UpdateBook( DisplayBookForAdmin book, int bookId);

        public Task<string> DeleteBook( int id);

        public Task<List<DisplayBookForMember>> GetAvailableBooks();

        public Task<(BookLoan Loan, string Message)> BorrowBook( DisplayBookForMember book);

        public Task<(BookLoan Loan, string Message)> ReturnBook (DisplayBookForMember book);

        public Task<BookLoan> AddBookLoan(int userId , AddBookLoanDTO bookLoan); 

    }
}
