namespace JahezTask.Application.Interfaces.Services
{
    public interface INotificationReminderService
    {
        Task CheckDelayedBooks();

       void AddNotificationRecord (int userId ,int BookId, string message);
    }
}
