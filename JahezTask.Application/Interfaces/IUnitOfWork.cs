using JahezTask.Application.Interfaces.Repositories;

namespace JahezTask.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }
        IBookLoanRepository BookLoanRepository { get; }
        INotificationRepository NotificationRepository { get; }
        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Save();
        Task SaveAsync();

    }
}
