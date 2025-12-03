using JahezTask.Application.Interfaces.Repositories;

namespace JahezTask.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }
        IBookLoanRepository BookLoanRepository { get; }
        INotificationRepository NotificationRepository { get; }
        // Transaction management
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default);

    }
}
