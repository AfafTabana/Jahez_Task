using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.DTOs.BookLoan.Commands.AddBook;
using JahezTask.Application.Features.BookLoan.Commands.BorrowBook;
using JahezTask.Application.Features.BookLoan.Commands.ReturnBook;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Application.Services;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookLendingTest.Features.BookLoan
{
    public class BorrowBookHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly BorrowBookCommandHandler _handler;
        private readonly ILogger<BorrowBookCommandHandler> _logger;
        private readonly IBookLoanRepository _bookLoanRepository;
        private readonly IUserContext _userContext;
        public BorrowBookHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _logger = Substitute.For<ILogger<BorrowBookCommandHandler>>();
            _userContext = Substitute.For<IUserContext>();
            _bookLoanRepository = Substitute.For<IBookLoanRepository>();
            _handler = new BorrowBookCommandHandler(_mapper , _logger , _userContext , _bookRepository , _bookLoanRepository);

        }

        [Fact]
        public async Task Handle_ShouldSucceed_WhenUserCanBorrowAndBookAvailable()
        {
            // Arrange
            int userId = 1;
            int bookId = 1;
            var displayBook = new BorrowBookCommand
            {
                Id = bookId,
                Title = "Borrowable Book"
            };

            var book = new JahezTask.Domain.Entities.Book
            {
                Id = bookId,
                Title = displayBook.Title,
                IsAvailable = true
            };

            var bookLoan = new JahezTask.Domain.Entities.BookLoan
            {
                Id = 1,
                UserId = userId,
                BookId = book.Id,
                Status = (int)LoanStatus.Borrowed
            };

            _userContext.GetCurrentUserId().Returns(userId.ToString());
            _bookLoanRepository.CanBorrow(userId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);
            _mapper.Map<JahezTask.Domain.Entities.BookLoan>(Arg.Any<AddBookLoanCommand>()).Returns(bookLoan);
            // These are required because your handler calls them
            _bookRepository.BeginTransactionAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _bookRepository.SaveAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _bookRepository.CommitTransactionAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
            // Act
            var result = await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            result.Loan.Should().NotBeNull();
            result.Message.Should().Be("Book has been borrowed successfully.");
            _bookRepository.Received(1).Update(Arg.Is<JahezTask.Domain.Entities.Book>(b => (bool)!b.IsAvailable));
            await _bookRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
            _bookLoanRepository.Received(1).Add(Arg.Is<JahezTask.Domain.Entities.BookLoan>(bl =>
            bl.UserId == userId &&
            bl.BookId == bookId &&
             bl.Status == (int)LoanStatus.Borrowed));
            await _bookRepository.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenUserReachedBorrowingLimit()
        {
            // Arrange
            int userId = 1;
            var displayBook = new BorrowBookCommand
            {
                Id = 1,
                Title = "Book"
            };

            _userContext.GetCurrentUserId().Returns(userId.ToString());
            _bookLoanRepository.CanBorrow(userId, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("Cannot borrow the book. You have reached the borrowing limit.");
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenBookNotAvailable()
        {
            // Arrange
            int userId = 1;
            int bookId = 1;
            var displayBook = new BorrowBookCommand
            {
                Id = bookId,
                Title = "Unavailable Book"
            };

            var book = new JahezTask.Domain.Entities.Book
            {
                Id = bookId,
                Title = displayBook.Title,
                IsAvailable = false
            };

            _userContext.GetCurrentUserId().Returns(userId.ToString());
            _bookLoanRepository.CanBorrow(userId, Arg.Any<CancellationToken>()).Returns(true);
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

            // Act
            var result = await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("Cannot borrow the book, the book is not available.");
        }
    }
}
