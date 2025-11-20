using JahezTask.Application.Interfaces.Repositories;

namespace JahezTask.Application.Interfaces
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
