using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using JahezTask.Persistence.Data;

namespace JahezTask.Persistence.Repositories
{
    public class BookLoanRepository : GenericRepository<BookLoan>, IBookLoanRepository
    {
        private readonly AppDbContext appDbContext;
        public BookLoanRepository(AppDbContext context) : base(context)
        {

            appDbContext = context;
        }
        public List<BookLoan> GetBookLoanByUserId(int userId)
        {
            List<BookLoan> AllUserLoan = appDbContext.BookLoans.Where(c => c.UserId == userId).ToList();
            return AllUserLoan;

        }

        public bool CanBorrow(int userId)
        {
            List<BookLoan> AllUsserLoan = GetBookLoanByUserId(userId);
            foreach (var item in AllUsserLoan)
            {

                if (item.Status != (int)LoanStatus.Returned)
                {
                    return false;
                }

            }

            return true;
        }

        public BookLoan GetBookLoanRecord(int userId, int BookId)
        {
            BookLoan BookLoanRecord = appDbContext.BookLoans.FirstOrDefault(c => c.UserId == userId && c.BookId == BookId);
            return BookLoanRecord;
        }
    }
}
