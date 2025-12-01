namespace JahezTask.Application.Interfaces.Services
{
    public interface INotificationReminderService
    {
        Task CheckDelayedBooks();

       void AddNotificationRecord (int userId ,int bookId, string message);
    }
}
