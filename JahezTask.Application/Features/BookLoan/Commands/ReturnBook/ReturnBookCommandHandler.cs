using AutoMapper;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Features.BookLoan.Commands.ReturnBook
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, (JahezTask.Domain.Entities.BookLoan Loan, string Message)>
    {
        private readonly IMapper mapper;

        private readonly ILogger<ReturnBookCommandHandler> logger;

        private readonly IUserContext userContext;

        private readonly IBookRepository bookRepository;

        private readonly IBookLoanRepository bookLoanRepository;
        public ReturnBookCommandHandler(IMapper mapper, ILogger<ReturnBookCommandHandler> logger, IUserContext userContext, IBookRepository bookRepository, IBookLoanRepository bookLoanRepository)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.userContext = userContext;
            this.bookRepository = bookRepository;
            this.bookLoanRepository = bookLoanRepository;
        }
        public async Task<(Domain.Entities.BookLoan Loan, string Message)> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
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

                // 1. Get book
                Book ReturnedBook = await bookRepository.GetByIdAsync(request.Id, cancellationToken);


                if (ReturnedBook == null)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "Book not found.");
                }
                // 2. Get loan record
                JahezTask.Domain.Entities.BookLoan ReturnedBookLoanRecord = await bookLoanRepository.GetBookLoanRecord(userId, ReturnedBook.Id, cancellationToken);
                if (ReturnedBookLoanRecord == null)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "No loan record found for this book and user.");
                }
                // 3. Check if already returned
                if (ReturnedBookLoanRecord.Status == (int)LoanStatus.Returned)
                {
                    await bookRepository.RollbackTransactionAsync();
                    return (null, "This book has already been returned.");
                }
                // 4. Update book availability
                ReturnedBook.IsAvailable = true;
                bookRepository.Update(ReturnedBook);

                // 5. Update loan record

                ReturnedBookLoanRecord.Status = (int)LoanStatus.Returned;
                ReturnedBookLoanRecord.ReturnDate = DateTime.UtcNow;
                bookLoanRepository.Update(ReturnedBookLoanRecord);

                // 7. Save all changes atomically
                await bookRepository.SaveAsync(cancellationToken);

                // 8. Commit transaction
                await bookRepository.CommitTransactionAsync(cancellationToken);

                logger.LogInformation(
                    "Book {BookId} successfully returned by user {UserId}. Loan ID: {LoanId}",
                    ReturnedBook.Id, userId, ReturnedBookLoanRecord.Id);

                return (ReturnedBookLoanRecord, "Book has been returned successfully.");


            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await bookRepository.RollbackTransactionAsync();
                logger.LogError(ex,
                   "Error occurred while returning book {BookId} for user {UserId}",
                   request.Id, userId);

                return (null, "An error occurred while processing the book return.");
            }
        }


    }
}

