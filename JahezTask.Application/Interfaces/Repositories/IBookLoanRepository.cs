using JahezTask.Domain.Entities;

namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IBookLoanRepository  : IGenericRepository<BookLoan>
    {
        public Task<IEnumerable<BookLoan>> GetBookLoanByUserId (int userId , CancellationToken cancellationToken = default);

        public Task<bool> CanBorrow (int userId , CancellationToken cancellationToken = default);

        public Task<BookLoan> GetBookLoanRecord(int userId, int bookId , CancellationToken cancellationToken = default);
    }
}
