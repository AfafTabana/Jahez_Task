using AutoMapper;
using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.DTOs.BookLoan;
using Jahez_Task.Enums;
using Jahez_Task.Models;
using Jahez_Task.UnitOfWork;
using Jahez_Task.Enums;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Jahez_Task.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly unitOfWork unitOfWork;

        private readonly IMapper mapper;

        public BookService(unitOfWork unitOfWork, IMapper mapper)
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

        public void UpdateBook(DIsplayBook book)
        {
            if (book != null)
            {
                Book Book = mapper.Map<Book>(book);
                unitOfWork.BookRepository.Update(Book);
                unitOfWork.Save();

            }

        }

        public async Task DeleteBook(int Id)
        {
            if (await unitOfWork.BookRepository.IsExist(Id))
            {
                unitOfWork.BookRepository.Delete(Id);
                unitOfWork.SaveAsync();

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

        public void BorrowBook(int UserId , DisplayBook book)
        {

            bool CanBorrow = unitOfWork.BookLoanRepository.CanBorrow(UserId);

            if (book != null && CanBorrow == true) { 
                
                Book BorrowedBook = unitOfWork.BookRepository.GetBookByTitle(book.Title);
                if (BorrowedBook.IsAvailable == true) {

                    BorrowedBook.IsAvailable = false;
                    unitOfWork.BookRepository.Update(BorrowedBook);
                    unitOfWork.Save();
                    AddBookLoanDTO BookLOanRecord = new AddBookLoanDTO()
                    {
                        UserId = UserId,
                        BookId = BorrowedBook.Id ,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(7) ,
                        CreatedAt = DateTime.Now,
                        Status = (int)LoanStatus.Borrowed

                    };

                    AddBookLoan(UserId, BookLOanRecord);
                }
 
            
            }

        }

        public void AddBookLoan(int UserId, AddBookLoanDTO BookLoan)
        {
            BookLoan BookLoanRecord = mapper.Map<BookLoan>(BookLoan);
            unitOfWork.BookLoanRepository.Add(BookLoanRecord);
            unitOfWork.Save();
        }

        public async Task ReturnBook(int UserId , DisplayBook Book)
        {
            if (Book != null)
            {
                Book ReturnedBook = unitOfWork.BookRepository.GetBookByTitle(Book.Title);
                ReturnedBook.IsAvailable = true;
                unitOfWork.BookRepository.Update(ReturnedBook);
                unitOfWork.Save();

                BookLoan ReturnedBookLoanRecord = unitOfWork.BookLoanRepository.GetBookLoanRecord(UserId, ReturnedBook.Id);
                ReturnedBookLoanRecord.Status = (int)LoanStatus.Returned;
                ReturnedBookLoanRecord.ReturnDate = DateTime.Now;
                unitOfWork.BookLoanRepository.Update(ReturnedBookLoanRecord);
                unitOfWork.SaveAsync();

            }
            
        }
    }
}
