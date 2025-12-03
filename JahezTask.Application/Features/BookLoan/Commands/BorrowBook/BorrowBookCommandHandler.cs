using AutoMapper;
using JahezTask.Application.DTOs.BookLoan.Commands.AddBook;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Application.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Features.BookLoan.Commands.BorrowBook
{
    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, (JahezTask.Domain.Entities.BookLoan Loan, string Message)>
    {
        private readonly IMapper mapper;

        private readonly ILogger<BorrowBookCommandHandler> logger;

        private readonly IUserContext userContext;

        private readonly IBookRepository bookRepository;

        private readonly IBookLoanRepository bookLoanRepository;

        public BorrowBookCommandHandler(
            IMapper mapper,
            ILogger<BorrowBookCommandHandler> logger,
            IUserContext userContext,
            IBookRepository bookRepository,
            IBookLoanRepository bookLoanRepository)

        {
            this.mapper = mapper;
            this.logger = logger;
            this.userContext = userContext;
            this.bookRepository = bookRepository;
            this.bookLoanRepository = bookLoanRepository;
        }
        public async Task<(Domain.Entities.BookLoan Loan, string Message)> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return (null, "Invalid book data.");
            }

            // Get current user from authentication context
            var userIdString = userContext.GetCurrentUserId();

            if (string.IsNullOrEmpty(userIdString))
            {
                return (null, "User not authenticated.");
            }
            if (!int.TryParse(userIdString, out int userId))
            {
                return (null, "Invalid user ID format.");
            }

            // Begin transaction to ensure atomicity
            await bookRepository.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Check if user can borrow
                bool canBorrow = await bookLoanRepository.CanBorrow(userId, cancellationToken);
                if (!canBorrow)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "Cannot borrow the book. You have reached the borrowing limit.");
                }

                // 2. Get and verify book availability
                var borrowedBook = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
                if (borrowedBook == null)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "Book not found.");
                }

                if ((bool)!borrowedBook.IsAvailable)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "Cannot borrow the book, the book is not available.");
                }

                // 3. Update book availability
                borrowedBook.IsAvailable = false;
                bookRepository.Update(borrowedBook);
                // 4. Create book loan record
                var bookLoanDto = new AddBookLoanCommand
                {
                    UserId = userId,
                    BookId = borrowedBook.Id,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    Status = (int)LoanStatus.Borrowed
                };

                var bookLoanRecord = mapper.Map<JahezTask.Domain.Entities.BookLoan>(bookLoanDto);
                bookLoanRepository.Add(bookLoanRecord);


                // 5. Save all changes atomically
                await bookRepository.SaveAsync(cancellationToken);

                // 6. Commit transaction
                await bookRepository.CommitTransactionAsync(cancellationToken);

                logger.LogInformation(
                    "Book {BookId} successfully borrowed by user {UserId}. Loan ID: {LoanId}",
                    borrowedBook.Id, userId, bookLoanRecord.Id);

                return (bookLoanRecord, "Book has been borrowed successfully.");


            }
            catch (Exception ex)
            {
                // Rollback on any error
                await bookRepository.RollbackTransactionAsync();

                logger.LogError(ex,
                    "Error occurred while borrowing book {BookId} for user {UserId}",
                    request.Id, userId);

                return (null, "An error occurred while processing the book loan.");
            }

        }
    }
}
