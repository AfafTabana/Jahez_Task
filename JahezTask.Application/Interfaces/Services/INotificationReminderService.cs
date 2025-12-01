namespace JahezTask.Application.Interfaces.Services
{
    public interface INotificationReminderService
    {
        Task CheckDelayedBooksForHangfireAsync(CancellationToken cancellationToken = default);
        Task CheckDelayedBooks(CancellationToken cancellationToken = default);

       Task AddNotificationRecord (int userId ,int bookId, string message , CancellationToken cancellationToken = default);
    }
}
