using Castle.Core.Logging;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JahezTask.Application.Services
{
    public class NotificationReminderService : INotificationReminderService
    {
        private readonly IUnitOfWork UnitOfWork;

        private readonly ILogger<NotificationReminderService> Logger;


        public NotificationReminderService(IUnitOfWork unitOfWork , ILogger<NotificationReminderService> _logger)
        {
            UnitOfWork = unitOfWork;
            Logger = _logger;
        }
        public async Task CheckDelayedBooks()
        {
            IEnumerable<BookLoan> AllBookLoans = await UnitOfWork.BookLoanRepository.GetAllAsync();
            foreach (var loan in AllBookLoans)
            {
                if (loan.DueDate < DateTime.Now && loan.Status != (int)LoanStatus.Returned)
                {
                    //log the reminder action
                    string message = $"Reminder: Book with ID {loan.BookId} borrowed by User ID {loan.UserId} is overdue since {loan.DueDate.ToShortDateString()}.";
                    Logger.LogInformation(message);

                    //update loan status to overdue
                    loan.Status = (int)LoanStatus.Overdue;
                    UnitOfWork.BookLoanRepository.Update(loan);
                    UnitOfWork.Save();

                   //Add Notification Record
                    AddNotificationRecord(loan.UserId, loan.Id, message);

                }
            }

        }

        public void AddNotificationRecord(int userId, int BookId, string message )
        {
          OverDueNotification notification = new OverDueNotification()
            {
                UserId = userId,
                BookLoanId = BookId,
                NotificationDate = DateTime.Now,
                NotificationType = (int)NotificationType.Log,
                NotificationMessage = message,
                IsSent = true,
                SentAt = DateTime.Now

          };
            UnitOfWork.NotificationRepository.Add(notification);
            UnitOfWork.Save();
        }


    }
}
