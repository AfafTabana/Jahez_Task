using Jahez_Task.Repository.BookLoanRepo;
using Jahez_Task.Repository.BookRepo;
using Jahez_Task.Repository.NotificationRepo;

namespace Jahez_Task.UnitOfWorkFolder
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }
        IBookLoanRepository BookLoanRepository { get; }
        INotificationRepository NotificationRepository { get; }

        void Save();
        Task SaveAsync();

    }
}
