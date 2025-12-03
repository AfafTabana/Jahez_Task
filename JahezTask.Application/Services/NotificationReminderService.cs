using Castle.Core.Logging;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Microsoft.Extensions.Hosting;


namespace JahezTask.Application.Services
{
    public class NotificationReminderService : INotificationReminderService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<NotificationReminderService> logger;

       


        public NotificationReminderService(IUnitOfWork unitOfWork , ILogger<NotificationReminderService> _logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = _logger;
            
        }
        
        public async Task CheckDelayedBooksForHangfireAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Create a linked token with timeout (e.g., 10 minutes max)
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);

                await CheckDelayedBooks(linkedCts.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogWarning(ex, "Hangfire job was cancelled or timed out");
                throw; // Re-throw so Hangfire can see job failed
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Hangfire job");
                throw;
            }
        }
        public async Task CheckDelayedBooks(CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Starting overdue books check...");

                var overdueLoans = await unitOfWork.BookLoanRepository
                    .GetQueryable()
                    .Where(loan => loan.DueDate < DateTime.Now
                                  && loan.Status != (int)LoanStatus.Returned
                                  && loan.Status != (int)LoanStatus.Overdue)
                    .ToListAsync(cancellationToken);  

                logger.LogInformation("Found {Count} overdue loans to process.", overdueLoans.Count);

                if (!overdueLoans.Any())
                {
                    logger.LogInformation("No overdue loans found.");
                    return;
                }

                
                foreach (var loan in overdueLoans)
                {
                    // Check for cancellation periodically
                    cancellationToken.ThrowIfCancellationRequested();

                    // Log the reminder action
                    string message = $"Reminder: Book with ID {loan.BookId} borrowed by User ID {loan.UserId} is overdue since {loan.DueDate:yyyy-MM-dd}.";
                    logger.LogInformation(message);

                    // Update loan status to overdue
                    loan.Status = (int)LoanStatus.Overdue;
                    unitOfWork.BookLoanRepository.Update(loan);

                    // Add Notification Record ASYNC
                    await AddNotificationRecord(loan.UserId, loan.Id, message, cancellationToken);

                }

                if (overdueLoans.Any())
                {
                    await unitOfWork.SaveAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Overdue books check was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking delayed books");
                throw;
            }
        }
        

        

        public async Task AddNotificationRecord(int userId, int BookId, string message, CancellationToken cancellationToken = default)
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
            unitOfWork.NotificationRepository.Add(notification);
          
        }


    }
}
