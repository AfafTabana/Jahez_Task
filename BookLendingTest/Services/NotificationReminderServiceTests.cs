using FluentAssertions;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Application.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookLendingTest.Services
{
    public class NotificationReminderServiceTests
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IBookLoanRepository _bookLoanRepository;
        private readonly ILogger<NotificationReminderService> _logger;
        private readonly NotificationReminderService _notificationService;

        public NotificationReminderServiceTests()
        {
            _notificationRepository = Substitute.For<INotificationRepository>();
            _bookLoanRepository = Substitute.For<IBookLoanRepository>();
            _logger = Substitute.For<ILogger<NotificationReminderService>>();

            _notificationService = new NotificationReminderService(
                _notificationRepository,
                _logger,
                _bookLoanRepository);
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

            
            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            _bookLoanRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert - Verify both loans were updated to Overdue
            _bookLoanRepository.Received(2).Update(
                Arg.Is<BookLoan>(loan => loan.Status == (int)LoanStatus.Overdue));

            // Verify SaveAsync was called on bookLoanRepository once (after all updates)
            await _bookLoanRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());

            // Verify notifications were created
            _notificationRepository.Received(2).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId > 0 &&
                    n.BookLoanId > 0 &&
                    n.NotificationType == (int)NotificationType.Log &&
                    n.IsSent == true));

            // Verify SaveAsync was called on notificationRepository (once per notification = 2 times)
            await _notificationRepository.Received(2).SaveAsync(Arg.Any<CancellationToken>());
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

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
            _bookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
            _notificationRepository.DidNotReceive().Add(Arg.Any<OverDueNotification>());
            await _bookLoanRepository.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
            await _notificationRepository.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
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
                    Status = (int)LoanStatus.Overdue 
                }
            };

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert
            _bookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
            _notificationRepository.DidNotReceive().Add(Arg.Any<OverDueNotification>());
        }

        [Fact]
        public async Task CheckDelayedBooks_WithEmptyList_ShouldNotThrowException()
        {
            // Arrange
            var bookLoans = new List<BookLoan>();

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            // Act
            var act = async () => await _notificationService.CheckDelayedBooks();

            // Assert
            await act.Should().NotThrowAsync();
            _bookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
        }

        [Fact]
        public async Task CheckDelayedBooks_ShouldOnlyProcessBorrowedStatus()
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
                    Status = (int)LoanStatus.Borrowed // Should be processed
                },
                new BookLoan
                {
                    Id = 2,
                    UserId = 2,
                    BookId = 20,
                    DueDate = overdueDate,
                    Status = (int)LoanStatus.Returned // Should NOT be processed
                },
                new BookLoan
                {
                    Id = 3,
                    UserId = 3,
                    BookId = 30,
                    DueDate = overdueDate,
                    Status = (int)LoanStatus.Overdue // Should NOT be processed
                }
            };

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            _bookLoanRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.CheckDelayedBooks();

            // Assert - Only 1 loan should be updated (the Borrowed one)
            _bookLoanRepository.Received(1).Update(
                Arg.Is<BookLoan>(loan =>
                    loan.Id == 1 &&
                    loan.Status == (int)LoanStatus.Overdue));

            _notificationRepository.Received(1).Add(Arg.Any<OverDueNotification>());
            await _notificationRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
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

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            // Assert
            _notificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId &&
                    n.NotificationMessage == message &&
                    n.NotificationType == (int)NotificationType.Log &&
                    n.IsSent == true));

            await _notificationRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AddNotificationRecord_ShouldSetCorrectTimestamps()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "Test notification";
            var beforeCall = DateTime.Now;

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            var afterCall = DateTime.Now;

            // Assert
            _notificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.SentAt >= beforeCall &&
                    n.SentAt <= afterCall &&
                    n.NotificationDate >= beforeCall &&
                    n.NotificationDate <= afterCall));
        }

        [Fact]
        public async Task AddNotificationRecord_WithCancellationToken_ShouldWork()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "Test notification message";
            var cancellationToken = new CancellationToken();

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message, cancellationToken);

            // Assert
            _notificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId &&
                    n.NotificationMessage == message));

            await _notificationRepository.Received(1).SaveAsync(cancellationToken);
        }

        [Fact]
        public async Task AddNotificationRecord_WithEmptyMessage_ShouldStillWork()
        {
            // Arrange
            int userId = 1;
            int bookLoanId = 10;
            string message = "";

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            // Assert
            _notificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n =>
                    n.UserId == userId &&
                    n.BookLoanId == bookLoanId &&
                    n.NotificationMessage == ""));
        }

        [Fact]
        public async Task AddNotificationRecord_ShouldSetIsSentToTrue()
        {
            // Arrange
            int userId = 5;
            int bookLoanId = 50;
            string message = "Overdue reminder";

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.AddNotificationRecord(userId, bookLoanId, message);

            // Assert
            _notificationRepository.Received(1).Add(
                Arg.Is<OverDueNotification>(n => n.IsSent == true));
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

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            _bookLoanRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.CheckDelayedBooksForHangfireAsync();

            // Assert
            _bookLoanRepository.Received(1).Update(
                Arg.Is<BookLoan>(loan => loan.Status == (int)LoanStatus.Overdue));

            await _bookLoanRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());

            _notificationRepository.Received(1).Add(Arg.Any<OverDueNotification>());
            await _notificationRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckDelayedBooksForHangfireAsync_WithNoOverdueBooks_ShouldNotUpdateOrNotify()
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
                    DueDate = futureDate,
                    Status = (int)LoanStatus.Borrowed
                }
            };

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            // Act
            await _notificationService.CheckDelayedBooksForHangfireAsync();

            // Assert
            _bookLoanRepository.DidNotReceive().Update(Arg.Any<BookLoan>());
            _notificationRepository.DidNotReceive().Add(Arg.Any<OverDueNotification>());
        }

        [Fact]
        public async Task CheckDelayedBooksForHangfireAsync_ShouldHandleMultipleOverdueBooks()
        {
            // Arrange
            var overdueDate = DateTime.Now.AddDays(-10);
            var bookLoans = new List<BookLoan>
            {
                new BookLoan { Id = 1, UserId = 1, BookId = 10, DueDate = overdueDate, Status = (int)LoanStatus.Borrowed },
                new BookLoan { Id = 2, UserId = 2, BookId = 20, DueDate = overdueDate, Status = (int)LoanStatus.Borrowed },
                new BookLoan { Id = 3, UserId = 3, BookId = 30, DueDate = overdueDate, Status = (int)LoanStatus.Borrowed }
            };

            _bookLoanRepository.GetQueryable()
                .Returns(bookLoans.ToAsyncQueryable());

            _bookLoanRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _notificationRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.CheckDelayedBooksForHangfireAsync();

            // Assert
            _bookLoanRepository.Received(3).Update(Arg.Any<BookLoan>());
            _notificationRepository.Received(3).Add(Arg.Any<OverDueNotification>());
            await _bookLoanRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
            await _notificationRepository.Received(3).SaveAsync(Arg.Any<CancellationToken>());
        }

        #endregion

    }
}