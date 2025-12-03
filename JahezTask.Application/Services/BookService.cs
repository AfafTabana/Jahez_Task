using AutoMapper;
using JahezTask.Application.DTOs.Book;
using JahezTask.Application.DTOs.BookLoan;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Application.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Jahez_Task.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly ILogger<BookService> logger;

        private readonly IUserContext userContext;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper , ILogger<BookService> logger, IUserContext userContext)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
            this.userContext = userContext;
        }

        public async Task<DisplayBookForMember> GetById(int id, CancellationToken cancellationToken = default)
        {
            if (!await unitOfWork.BookRepository.IsExist(id , cancellationToken))
            {

                return null;

            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id , cancellationToken);
            DisplayBookForMember _Book = mapper.Map<DisplayBookForMember>(Book);

            return _Book;

        }

        public async Task<DisplayBookForAdmin> GetByIdForAdmin(int id , CancellationToken cancellationToken = default)
        {
            if (!await unitOfWork.BookRepository.IsExist(id , cancellationToken))
            {
                return null;
            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id , cancellationToken);
            DisplayBookForAdmin _book = mapper.Map<DisplayBookForAdmin>(Book);

            return _book;
        }

        public async Task<IEnumerable<DisplayBookForMember>> GetAll(CancellationToken cancellationToken = default)
        {

            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync(cancellationToken);
            IEnumerable<DisplayBookForMember> _Books = mapper.Map<IEnumerable<DisplayBookForMember>>(Books);

            return _Books;

        }

        public async Task<IEnumerable<DisplayBookForAdmin>> GetAllBookForAdmin(CancellationToken cancellationToken = default)
        {
            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync(cancellationToken);
            IEnumerable<DisplayBookForAdmin> _Books = mapper.Map<IEnumerable<DisplayBookForAdmin>>(Books);

            return _Books;


        }

        public async Task AddBook(DisplayBookForAdmin book , CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (book != null)
            {
                Book Book = mapper.Map<Book>(book);
                unitOfWork.BookRepository.Add(Book);
               await unitOfWork.SaveAsync(cancellationToken);

            }


        }

        public async Task UpdateBook(DisplayBookForAdmin book , int bookId , CancellationToken cancellationToken = default)
        {
            var existing = await unitOfWork.BookRepository.GetByIdAsync(bookId , cancellationToken);

            if (book != null && existing!= null)
            {
                Book Book = mapper.Map<Book>(book);
                Book.Id = bookId;
                unitOfWork.BookRepository.Update(Book);
                await unitOfWork.SaveAsync(cancellationToken);

            }

        }

        public async Task<string> DeleteBook(int id , CancellationToken cancellationToken = default)
        {
            string Message = "";
            if (await unitOfWork.BookRepository.IsExist(id, cancellationToken))
            {
                unitOfWork.BookRepository.Delete(id);
                await unitOfWork.SaveAsync(cancellationToken);
                Message = "Book Deleted Successfully";
                return Message;
            }else
            {
                Message = "Book Not Found";
                return Message;

            }


        }

        public async Task<List<DisplayBookForMember>> GetAvailableBooks(CancellationToken cancellationToken = default)
        {
            IEnumerable<Book> AllBooks = await unitOfWork.BookRepository.GetAllAsync(cancellationToken);
            List<DisplayBookForMember> _AllBooks = new List<DisplayBookForMember>();
            foreach(Book book in AllBooks)
            {
                if (book.IsAvailable ==  true)
                {
                    _AllBooks.Add(mapper.Map<DisplayBookForMember>(book));
                
                }
            }

            return _AllBooks;
        }

        public async Task<(BookLoan Loan, string Message)> BorrowBook( DisplayBookForMember book , CancellationToken cancellationToken = default)
        {
            if (book == null)
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
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Check if user can borrow
                bool canBorrow = await unitOfWork.BookLoanRepository.CanBorrow(userId , cancellationToken);
                if (!canBorrow)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (null, "Cannot borrow the book. You have reached the borrowing limit.");
                }

                // 2. Get and verify book availability
                var borrowedBook = await unitOfWork.BookRepository.GetByIdAsync(book.Id , cancellationToken);
                if (borrowedBook == null)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (null, "Book not found.");
                }

                if ((bool)!borrowedBook.IsAvailable)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (null, "Cannot borrow the book, the book is not available.");
                }

                // 3. Update book availability
                borrowedBook.IsAvailable = false;
                unitOfWork.BookRepository.Update(borrowedBook);
                // 4. Create book loan record
                var bookLoanDto = new AddBookLoanDTO
                {
                    UserId = userId,
                    BookId = borrowedBook.Id,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    Status = (int)LoanStatus.Borrowed
                };

                var bookLoanRecord = mapper.Map<BookLoan>(bookLoanDto);
                unitOfWork.BookLoanRepository.Add(bookLoanRecord);


                // 5. Save all changes atomically
                await unitOfWork.SaveAsync(cancellationToken);

                // 6. Commit transaction
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation(
                    "Book {BookId} successfully borrowed by user {UserId}. Loan ID: {LoanId}",
                    borrowedBook.Id, userId, bookLoanRecord.Id);

                return (bookLoanRecord, "Book has been borrowed successfully.");


            }
            catch (Exception ex)
            {
                // Rollback on any error
                await unitOfWork.RollbackTransactionAsync();

                logger.LogError(ex,
                    "Error occurred while borrowing book {BookId} for user {UserId}",
                    book.Id, userId);

                return (null, "An error occurred while processing the book loan.");
            }



        }

        public async Task<BookLoan> AddBookLoan(int userId, AddBookLoanDTO bookLoan , CancellationToken cancellationToken = default)
        {
            BookLoan BookLoanRecord = mapper.Map<BookLoan>(bookLoan);
            unitOfWork.BookLoanRepository.Add(BookLoanRecord);
            await unitOfWork.SaveAsync(cancellationToken);
            return BookLoanRecord;
        }

        public async Task<(BookLoan Loan, string Message)> ReturnBook(DisplayBookForMember book , CancellationToken cancellationToken = default)
        {
            if (book == null)
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
            await unitOfWork.BeginTransactionAsync(cancellationToken);

           
            try {

                // 1. Get book
                Book ReturnedBook = await unitOfWork.BookRepository.GetByIdAsync(book.Id , cancellationToken);


           if (ReturnedBook == null)
            {
                await unitOfWork.RollbackTransactionAsync();
                return (null, "Book not found.");
            }
                // 2. Get loan record
                BookLoan ReturnedBookLoanRecord = await unitOfWork.BookLoanRepository.GetBookLoanRecord(userId, ReturnedBook.Id , cancellationToken);
                if (ReturnedBookLoanRecord == null)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (null, "No loan record found for this book and user.");
                }
                // 3. Check if already returned
                if (ReturnedBookLoanRecord.Status == (int)LoanStatus.Returned)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (null, "This book has already been returned.");
                }
                // 4. Update book availability
                ReturnedBook.IsAvailable = true;
                unitOfWork.BookRepository.Update(ReturnedBook);
                
                // 5. Update loan record
                
                ReturnedBookLoanRecord.Status = (int)LoanStatus.Returned;
                ReturnedBookLoanRecord.ReturnDate = DateTime.UtcNow;
                unitOfWork.BookLoanRepository.Update(ReturnedBookLoanRecord);

                // 7. Save all changes atomically
                await unitOfWork.SaveAsync(cancellationToken);

                // 8. Commit transaction
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation(
                    "Book {BookId} successfully returned by user {UserId}. Loan ID: {LoanId}",
                    ReturnedBook.Id, userId,ReturnedBookLoanRecord.Id);

                return (ReturnedBookLoanRecord, "Book has been returned successfully.");


            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await unitOfWork.RollbackTransactionAsync();
                logger.LogError(ex,
                   "Error occurred while returning book {BookId} for user {UserId}",
                   book.Id, userId);

                return (null, "An error occurred while processing the book return.");
            }
        }



        }
    }



