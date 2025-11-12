using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;

namespace Jahez_Task.Repository.BookLoanRepo
{
    public interface IBookLoanRepository  : IGenericRepository<BookLoan>
    {
        public List<BookLoan> GetBookLoanByUserId (int userId);

        public bool CanBorrow (int userId);

        public BookLoan GetBookLoanRecord(int userId, int BookId);
    }
}
