using JahezTask.Application.DTOs.Book;
using JahezTask.Application.DTOs.BookLoan;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Interfaces.Services
{
    public interface IBookService
    {
        public Task<DisplayBookForMember> GetById(int id , CancellationToken cancellationToken = default);
        public Task<DisplayBookForAdmin> GetByIdForAdmin(int id , CancellationToken cancellationToken = default);
        public Task<IEnumerable<DisplayBookForMember>> GetAll(CancellationToken cancellationToken = default);
        public Task<IEnumerable<DisplayBookForAdmin>> GetAllBookForAdmin(CancellationToken cancellationToken = default);

        public Task AddBook(DisplayBookForAdmin book , CancellationToken cancellationToken = default);

        public Task UpdateBook( DisplayBookForAdmin book, int bookId  , CancellationToken cancellationToken = default);

        public Task<string> DeleteBook( int id , CancellationToken cancellationToken = default);

        public Task<List<DisplayBookForMember>> GetAvailableBooks(CancellationToken cancellationToken = default);

        public Task<(BookLoan Loan, string Message)> BorrowBook( DisplayBookForMember book , CancellationToken cancellationToken = default);

        public Task<(BookLoan Loan, string Message)> ReturnBook (DisplayBookForMember book , CancellationToken cancellationToken = default);

        public Task<BookLoan> AddBookLoan(int userId , AddBookLoanDTO bookLoan , CancellationToken cancellationToken = default); 

    }
}
