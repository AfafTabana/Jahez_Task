using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using JahezTask.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace JahezTask.Persistence.Repositories
{
    public class BookLoanRepository : GenericRepository<BookLoan>, IBookLoanRepository
    {
        private readonly AppDbContext appDbContext;
        public BookLoanRepository(AppDbContext context) : base(context)
        {

            appDbContext = context;
        }
        public async Task<IEnumerable<BookLoan>> GetBookLoanByUserId(int userId , CancellationToken cancellationToken = default)
        {
            List<BookLoan> AllUserLoan = await appDbContext.BookLoans.Where(c => c.UserId == userId).AsNoTracking().ToListAsync(cancellationToken);
            return AllUserLoan;

        }

        public async Task<bool> CanBorrow(int userId , CancellationToken cancellationToken = default)
        {
            IEnumerable<BookLoan> AllUsserLoan = await GetBookLoanByUserId(userId , cancellationToken);
            foreach (var item in AllUsserLoan)
            {

                if (item.Status != (int)LoanStatus.Returned)
                {
                    return false;
                }

            }

            return true;
        }

        public async Task<BookLoan> GetBookLoanRecord(int userId, int bookId , CancellationToken cancellationToken  = default)
        {
            BookLoan BookLoanRecord = await appDbContext.BookLoans.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId, cancellationToken);
            return BookLoanRecord;
        }
    }
}
