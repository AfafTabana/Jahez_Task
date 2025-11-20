using AutoMapper;
using JahezTask.Application.DTOs.Book;
using JahezTask.Application.DTOs.BookLoan;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using System.Threading.Tasks;

namespace Jahez_Task.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<DisplayBookForMember> GetById(int id)
        {
            if (!await unitOfWork.BookRepository.IsExist(id))
            {

                return null;

            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id);
            DisplayBookForMember _Book = mapper.Map<DisplayBookForMember>(Book);

            return _Book;

        }

        public async Task<DisplayBookForAdmin> GetByIdForAdmin(int id)
        {
            if (!await unitOfWork.BookRepository.IsExist(id))
            {
                return null;
            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id);
            DisplayBookForAdmin _book = mapper.Map<DisplayBookForAdmin>(Book);

            return _book;
        }

        public async Task<IEnumerable<DisplayBookForMember>> GetAll()
        {

            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync();
            IEnumerable<DisplayBookForMember> _Books = mapper.Map<IEnumerable<DisplayBookForMember>>(Books);

            return _Books;

        }

        public async Task<IEnumerable<DisplayBookForAdmin>> GetAllBookForAdmin()
        {
            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync();
            IEnumerable<DisplayBookForAdmin> _Books = mapper.Map<IEnumerable<DisplayBookForAdmin>>(Books);

            return _Books;


        }

        public void AddBook(DisplayBookForAdmin book)
        {

            if (book != null)
            {
                Book Book = mapper.Map<Book>(book);
                unitOfWork.BookRepository.Add(Book);
                unitOfWork.Save();

            }


        }

        public async Task UpdateBook(DisplayBookForAdmin book , int BookId)
        {
            var existing = await unitOfWork.BookRepository.GetByIdAsync(BookId);

            if (book != null && existing!= null)
            {
                Book Book = mapper.Map<Book>(book);
                Book.Id = BookId;
                unitOfWork.BookRepository.Update(Book);
                await unitOfWork.SaveAsync();

            }

        }

        public async Task<string> DeleteBook(int Id)
        {
            string Message = "";
            if (await unitOfWork.BookRepository.IsExist(Id))
            {
                unitOfWork.BookRepository.Delete(Id);
                unitOfWork.Save();
                Message = "Book Deleted Successfully";
                return Message;
            }else
            {
                Message = "Book Not Found";
                return Message;

            }


        }

        public async Task<List<DisplayBookForMember>> GetAvailableBooks()
        {
            IEnumerable<Book> AllBooks = await unitOfWork.BookRepository.GetAllAsync();
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

        public async Task<(BookLoan Loan, string Message)> BorrowBook(int UserId , DisplayBookForMember book)
        {

            bool CanBorrow = unitOfWork.BookLoanRepository.CanBorrow(UserId);

            if (book != null && CanBorrow) { 
                
                Book BorrowedBook = unitOfWork.BookRepository.GetBookByTitle(book.Title);
                if (BorrowedBook.IsAvailable == true) {

                    BorrowedBook.IsAvailable = false;
                    unitOfWork.BookRepository.Update(BorrowedBook);
                    
                    AddBookLoanDTO BookLOanRecord = new AddBookLoanDTO()
                    {
                        UserId = UserId,
                        BookId = BorrowedBook.Id ,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(7) ,
                        CreatedAt = DateTime.Now,
                        Status = (int)LoanStatus.Borrowed

                    };

                   BookLoan Loan = await AddBookLoan(UserId, BookLOanRecord);
                   return (Loan , "Book has been borrowed successfully.");


                }else
                {
                    return (null, "Cannot borrow the book , the book is not available.");

                }



            }

                return (null , "Cannot borrow the book. You  have reached the borrowing limit ");

        }

        public async Task<BookLoan> AddBookLoan(int UserId, AddBookLoanDTO BookLoan)
        {
            BookLoan BookLoanRecord = mapper.Map<BookLoan>(BookLoan);
            unitOfWork.BookLoanRepository.Add(BookLoanRecord);
            await unitOfWork.SaveAsync();
            return BookLoanRecord;
        }

        public async Task<(BookLoan Loan, string Message)> ReturnBook(int UserId , DisplayBookForMember Book)
        {
            if (Book != null)
            {
                Book ReturnedBook = unitOfWork.BookRepository.GetBookByTitle(Book.Title);
                ReturnedBook.IsAvailable = true;
                unitOfWork.BookRepository.Update(ReturnedBook);
                unitOfWork.Save();

                BookLoan ReturnedBookLoanRecord = unitOfWork.BookLoanRepository.GetBookLoanRecord(UserId, ReturnedBook.Id);
                if (ReturnedBookLoanRecord.Status != (int)LoanStatus.Returned)
                {
                    ReturnedBookLoanRecord.Status = (int)LoanStatus.Returned;
                    ReturnedBookLoanRecord.ReturnDate = DateTime.Now;
                    unitOfWork.BookLoanRepository.Update(ReturnedBookLoanRecord);
                    await unitOfWork.SaveAsync();
                    return (ReturnedBookLoanRecord, "Book has been Returned successfully.");
                }
                else
                {
                    return (null, "This book has already been returned.");
                }
               

            }

               return (null, "Invalid book data.");

        }
    }
}
