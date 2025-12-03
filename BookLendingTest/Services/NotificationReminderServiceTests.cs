using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Application.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JahezTask.Application.Interfaces.Repositories;

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

           
            var mockQueryable = bookLoans.ToAsyncQueryable();

            
            var mockBookLoanRepository = Substitute.For<IBookLoanRepository>();
            mockBookLoanRepository.GetQueryable().Returns(mockQueryable);
            _unitOfWork.BookLoanRepository.Returns(mockBookLoanRepository);

            var mockNotificationRepository = Substitute.For<INotificationRepository>();
            _unitOfWork.NotificationRepository.Returns(mockNotificationRepository);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
          
            _unitOfWork.BookLoanRepository.Received(2).Update(
                Arg.Is<BookLoan>(loan => loan.Status == (int)LoanStatus.Overdue));

           
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());

           
            _unitOfWork.NotificationRepository.Received(2).Add(
                Arg.Any<OverDueNotification>());

            
            // The service logs:
            // 1. "Starting overdue books check..."
            // 2. "Found 2 overdue loans to process."
            // 3. "Reminder: Book with ID 10 borrowed by User ID 1 is overdue since..."
            // 4. "Reminder: Book with ID 20 borrowed by User ID 2 is overdue since..."

            //Check for at least 4 Information log calls
            _logger.Received(4).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            // Check specifically for the reminder logs (2 of them)
            _logger.Received(2).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(state => state.ToString().Contains("Reminder")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }

        [Fact]
        public async Task CheckDelayedBooks_WithNoOverdueLoans_ShouldDoNothing()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(5);
            var bookLoans = new List<BookLoan>
    {
        new BookLoan
        {
            Id = 1,
            UserId = 1,
            BookId = 10,
            DueDate = futureDate, // Not overdue
            Status = (int)LoanStatus.Borrowed
        },
        new BookLoan
        {
            Id = 2,
            UserId = 2,
            BookId = 20,
            DueDate = futureDate, // Not overdue
            Status = (int)LoanStatus.Returned 
        }
    };

            
            var mockQueryable = bookLoans.ToAsyncQueryable();
            var mockBookLoanRepository = Substitute.For<IBookLoanRepository>();
            mockBookLoanRepository.GetQueryable().Returns(mockQueryable);
            _unitOfWork.BookLoanRepository.Returns(mockBookLoanRepository);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
            _unitOfWork.BookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
            _unitOfWork.NotificationRepository.DidNotReceive().Add(Arg.Any<OverDueNotification>());
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckDelayedBooks_WhenAlreadyOverdue_ShouldNotUpdateAgain()
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
            Status = (int)LoanStatus.Overdue // Already marked as overdue
        }
    };

           
            var mockQueryable = bookLoans.ToAsyncQueryable();
            var mockBookLoanRepository = Substitute.For<IBookLoanRepository>();
            mockBookLoanRepository.GetQueryable().Returns(mockQueryable);
            _unitOfWork.BookLoanRepository.Returns(mockBookLoanRepository);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
            _unitOfWork.BookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
            _unitOfWork.NotificationRepository.DidNotReceive().Add(Arg.Any<OverDueNotification>());
        }

        #endregion

        #region AddNotificationRecord Tests

        [Fact]
        public async Task AddNotificationRecord_ShouldCreateNotification()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "Test notification message";

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            // Assert
            _unitOfWork.NotificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId &&
                    n.NotificationMessage == message &&
                    n.NotificationType == (int)NotificationType.Log &&
                    n.IsSent == true &&
                    n.SentAt != null &&
                    n.NotificationDate != null));

         
            _unitOfWork.DidNotReceive().Save();
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AddNotificationRecord_WithCancellationToken_ShouldWork()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "Test notification message";
            var cancellationToken = new CancellationToken();

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message, cancellationToken);

            // Assert
            _unitOfWork.NotificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId));
        }

        #endregion

        #region CheckDelayedBooksForHangfireAsync Tests

        [Fact]
        public async Task CheckDelayedBooksForHangfireAsync_ShouldCallCheckDelayedBooks()
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
        }
    };

            
            var mockQueryable = bookLoans.ToAsyncQueryable();
            var mockBookLoanRepository = Substitute.For<IBookLoanRepository>();
            mockBookLoanRepository.GetQueryable().Returns(mockQueryable);
            _unitOfWork.BookLoanRepository.Returns(mockBookLoanRepository);

            var mockNotificationRepository = Substitute.For<INotificationRepository>();
            _unitOfWork.NotificationRepository.Returns(mockNotificationRepository);

            // Act
            await _notificationService.CheckDelayedBooksForHangfireAsync();

            // Assert
           
            _unitOfWork.BookLoanRepository.Received(1).Update(
                Arg.Is<BookLoan>(loan => loan.Status == (int)LoanStatus.Overdue));
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        #endregion
    }
}