using JahezTask.Domain.Entities;

namespace JahezTask.Application.Interfaces.Repositories
{
    public interface IBookLoanRepository  : IGenericRepository<BookLoan>
    {
        public List<BookLoan> GetBookLoanByUserId (int userId);

        public bool CanBorrow (int userId);

        public BookLoan GetBookLoanRecord(int userId, int bookId);
    }
}
