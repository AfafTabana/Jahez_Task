using AutoMapper;
using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.DTOs.BookLoan;
using Jahez_Task.Enums;
using Jahez_Task.Enums;
using Jahez_Task.Models;
using Jahez_Task.UnitOfWork;
using Jahez_Task.UnitOfWorkFolder;
using Microsoft.AspNetCore.Identity.UI.Services;
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

        public async Task<DisplayBook> GetById(int id)
        {
            if (!await unitOfWork.BookRepository.IsExist(id))
            {

                return null;

            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id);
            DisplayBook _Book = mapper.Map<DisplayBook>(Book);

            return _Book;

        }

        public async Task<DIsplayBook> GetByIdForAdmin(int id)
        {
            if (!await unitOfWork.BookRepository.IsExist(id))
            {
                return null;
            }

            var Book = await unitOfWork.BookRepository.GetByIdAsync(id);
            DIsplayBook _book = mapper.Map<DIsplayBook>(Book);

            return _book;
        }

        public async Task<IEnumerable<DisplayBook>> GetAll()
        {

            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync();
            IEnumerable<DisplayBook> _Books = mapper.Map<IEnumerable<DisplayBook>>(Books);

            return _Books;

        }

        public async Task<IEnumerable<DIsplayBook>> GetAllBookForAdmin()
        {
            IEnumerable<Book> Books = await unitOfWork.BookRepository.GetAllAsync();
            IEnumerable<DIsplayBook> _Books = mapper.Map<IEnumerable<DIsplayBook>>(Books);

            return _Books;


        }

        public void AddBook(DIsplayBook book)
        {

            if (book != null)
            {
                Book Book = mapper.Map<Book>(book);
                unitOfWork.BookRepository.Add(Book);
                unitOfWork.Save();

            }


        }

        public async Task UpdateBook(DIsplayBook book , int BookId)
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

        public async Task<List<DisplayBook>> GetAvailableBooks()
        {
            IEnumerable<Book> AllBooks = await unitOfWork.BookRepository.GetAllAsync();
            List<DisplayBook> _AllBooks = new List<DisplayBook>();
            foreach(Book book in AllBooks)
            {
                if (book.IsAvailable ==  true)
                {
                    _AllBooks.Add(mapper.Map<DisplayBook>(book));
                
                }
            }

            return _AllBooks;
        }

        public async Task<(BookLoan Loan, string Message)> BorrowBook(int UserId , DisplayBook book)
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

        public async Task<(BookLoan Loan, string Message)> ReturnBook(int UserId , DisplayBook Book)
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
