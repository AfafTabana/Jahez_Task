using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.Features.BookLoan.Commands.BorrowBook;
using JahezTask.Application.Features.BookLoan.Commands.ReturnBook;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Application.Services;
using JahezTask.Domain.Enums;
using JahezTask.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Features.BookLoan
{
    public class ReturnBookHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ReturnBookCommandHandler _handler;
        private readonly ILogger<ReturnBookCommandHandler> _logger;
        private readonly IBookLoanRepository _bookLoanRepository;
        private readonly IUserContext _userContext;

        public ReturnBookHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _logger = Substitute.For<ILogger<ReturnBookCommandHandler>>();
            _userContext = Substitute.For<IUserContext>();
            _bookLoanRepository = Substitute.For<IBookLoanRepository>();
            _handler = new ReturnBookCommandHandler(_mapper, _logger, _userContext, _bookRepository, _bookLoanRepository);

        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessfully_WhenBookIsValid()
        {
            // Arrange
            int userId = 1;
            int bookId = 1;
            var displayBook = new ReturnBookCommand
            {
                Id = bookId,
                Title = "Returned Book"
            };

            var book = new JahezTask.Domain.Entities.Book
            {
                Id = bookId,
                Title = displayBook.Title,
                IsAvailable = false
            };

            var bookLoan = new JahezTask.Domain.Entities.BookLoan
            {
                Id = 1,
                UserId = userId,
                BookId = book.Id,
                Status = (int)LoanStatus.Borrowed
            };

            _userContext.GetCurrentUserId().Returns(userId.ToString());
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);
            _bookLoanRepository.GetBookLoanRecord(userId, bookId, Arg.Any<CancellationToken>()).Returns(bookLoan);

            // Act
            var result = await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            result.Loan.Should().NotBeNull();
            result.Message.Should().Be("Book has been returned successfully.");
            result.Loan.Status.Should().Be((int)LoanStatus.Returned);
            result.Loan.ReturnDate.Should().NotBeNull();
            _bookRepository.Received(1).Update(Arg.Is<JahezTask.Domain.Entities.Book>(b => (bool)b.IsAvailable));
        }

        [Fact]
        public async Task Handle_ShouldReturnAlreadyReturnedMessage_WhenBookAlreadyReturned()
        {
            // Arrange
            int userId = 1;
            int bookId = 1;
            var displayBook = new ReturnBookCommand
            {
                Id = bookId,  // Added Id property
                Title = "Already Returned Book"
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
                Status = (int)LoanStatus.Returned,
                ReturnDate = DateTime.Now.AddDays(-1)
            };

            _userContext.GetCurrentUserId().Returns(userId.ToString());
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);
            _bookLoanRepository.GetBookLoanRecord(userId, bookId, Arg.Any<CancellationToken>()).Returns(bookLoan);

            // Act
            var result = await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("This book has already been returned.");
        }
    }
}
