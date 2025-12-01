using Castle.Core.Logging;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;


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
            // Query only overdue loans directly from the database
            var overdueLoans = await UnitOfWork.BookLoanRepository
                .GetQueryable()
                .Where(loan => loan.DueDate < DateTime.Now
                              && loan.Status != (int)LoanStatus.Returned
                              && loan.Status != (int)LoanStatus.Overdue) 
                .ToListAsync();

            foreach (var loan in overdueLoans)
            {
                //log the reminder action
                string message = $"Reminder: Book with ID {loan.BookId} borrowed by User ID {loan.UserId} is overdue since {loan.DueDate.ToShortDateString()}.";
                Logger.LogInformation(message);

                //update loan status to overdue
                loan.Status = (int)LoanStatus.Overdue;
                UnitOfWork.BookLoanRepository.Update(loan);

                //Add Notification Record
                AddNotificationRecord(loan.UserId, loan.Id, message);
            }

            if (overdueLoans.Any())
            {
                await UnitOfWork.SaveAsync();
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
