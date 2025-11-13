namespace Jahez_Task.Services.NotificationService
{
    public interface INotificationReminderService
    {
        Task CheckDelayedBooks();

       void AddNotificationRecord (int userId ,int BookId, string message);
    }
}
