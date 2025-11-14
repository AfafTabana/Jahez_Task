using Jahez_Task.Enums;
using Jahez_Task.Models;
using Jahez_Task.Services.NotificationService;
using Jahez_Task.UnitOfWork;
using Jahez_Task.UnitOfWorkFolder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Services
{
    public class NotificationReminderServiceTests
    {
        private readonly INotificationReminderService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationReminderService> _logger;

        public NotificationReminderServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _logger = Substitute.For<ILogger<NotificationReminderService>>();
            _notificationService = new NotificationReminderService(_unitOfWork, _logger);
        }

        #region CheckDelayedBooks Tests

        [Fact]
        public async Task CheckDelayedBooks_WithOverdueLoans_ShouldUpdateStatusAndCreateNotifications()
        {
            // Arrange
            var overdueDate = DateTime.Now.AddDays(-5);
            var bookLoans = new List<BookLoan>
            {
                new BookLoan
                {
                    Id = 1,
                    UserId = 1,
                    BookId = 10,
                    DueDate = overdueDate,
                    Status = (int)LoanStatus.Borrowed
                },
                new BookLoan
                {
                    Id = 2,
                    UserId = 2,
                    BookId = 20,
                    DueDate = overdueDate,
                    Status = (int)LoanStatus.Borrowed
                }
            };

            _unitOfWork.BookLoanRepository.GetAllAsync().Returns(bookLoans);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
            
            _unitOfWork.BookLoanRepository.Received(2).Update(
                Arg.Is<BookLoan>(loan => loan.Status == (int)LoanStatus.Overdue));

           
            _unitOfWork.Received(4).Save(); 

            
            _unitOfWork.NotificationRepository.Received(2).Add(
                Arg.Any<OverDueNotification>());


            _logger.Received(2).Log(
             LogLevel.Information,
             Arg.Any<EventId>(),
             Arg.Is<object>(state =>
             state.ToString().Contains("Reminder") &&
             state.ToString().Contains("overdue")),
             Arg.Any<Exception>(),
             Arg.Any<Func<object, Exception?, string>>()
                );

        }


        #endregion

        #region AddNotificationRecord Tests
        [Fact]
        public void AddNotificationRecord_ShouldCreateAndSaveNotification()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "Test notification message";

            // Act
            _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            // Assert
            _unitOfWork.NotificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId &&
                    n.NotificationMessage == message &&
                    n.NotificationType == (int)NotificationType.Log &&
                    n.IsSent == true));

            _unitOfWork.Received(1).Save();
        }

        #endregion
    }
}
