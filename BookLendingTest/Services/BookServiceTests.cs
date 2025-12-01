using AutoMapper;
using FluentAssertions;
using Jahez_Task.Services.BookService;
using JahezTask.Application.DTOs.Book;
using JahezTask.Application.DTOs.BookLoan;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Services
{
    public class BookServiceTests
    {
        private readonly IBookService _bookService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> logger;
        private readonly IUserContext userContext;

        public BookServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            logger = Substitute.For<ILogger<BookService>>();
            userContext = Substitute.For<IUserContext>();
            _bookService = new BookService(_unitOfWork, _mapper , logger , userContext);
        }

        #region GetById Tests

        [Fact]
        public async Task GetById_BookExists_ReturnsDisplayBook()
        {
            // Arrange
            int bookId = 1;
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                Description = "Test Description",
                IsAvailable = true
            };
            var displayBook = new DisplayBookForMember
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description
            };
            _unitOfWork.BookRepository.IsExist(bookId).Returns(true);
            _unitOfWork.BookRepository.GetByIdAsync(bookId).Returns(Task.FromResult(book));
            _mapper.Map<DisplayBookForMember>(book).Returns(displayBook);
            // Act
            var result = await _bookService.GetById(bookId);
            // Assert
            Assert.NotNull(result);
            result.Title.Should().Be(book.Title);
            result.Author.Should().Be(book.Author);
           
        }

        [Fact]
        public async Task GetById_BookDoesNotExist_ReturnsNull()
        {
            // Arrange
            int bookId = 1;
            _unitOfWork.BookRepository.IsExist(bookId).Returns(false);
            // Act
            var result = await _bookService.GetById(bookId);
            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetByIdForAdmin Tests
        [Fact]
        public async Task GetByIdForAdmin_BookExists_ReturnsDIsplayBook()
        {
            // Arrange
            int bookId = 1;
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                Description = "Test Description",
                IsAvailable = true
            };
            var dIsplayBook = new DisplayBookForAdmin
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description ,
                IsAvailable= (bool) book.IsAvailable ,
                ISBN= book.ISBN

            };
            _unitOfWork.BookRepository.IsExist(bookId).Returns(true);
            _unitOfWork.BookRepository.GetByIdAsync(bookId).Returns(Task.FromResult(book));
            _mapper.Map<DisplayBookForAdmin>(book).Returns(dIsplayBook);
            // Act
            var result = await _bookService.GetByIdForAdmin(bookId);
            // Assert
            Assert.NotNull(result);
            result.Title.Should().Be(book.Title);
            result.Author.Should().Be(book.Author);
        }

        [Fact]
        public async Task GetByIdForAdmin_BookDoesNotExist_ReturnsNull()
        {
            // Arrange
            int bookId = 1;
            _unitOfWork.BookRepository.IsExist(bookId).Returns(false);
            // Act
            var result = await _bookService.GetByIdForAdmin(bookId);
            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAll
        [Fact]

        public async Task GetAll_ReturnsAllDisplayBooks()
        {
            //Arrange 
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1" },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", Description = "Description 2" }
            };
            _unitOfWork.BookRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Book>)books));
            var displayBooks = books.Select(b => new DisplayBookForMember
            {
                Title = b.Title,
                Author = b.Author,
                Description = b.Description
            }).ToList();
            _mapper.Map<IEnumerable<DisplayBookForMember>>(books).Returns(displayBooks);

            //Act
            var result = await _bookService.GetAll();
            //Assert
            Assert.NotNull(result);
            result.Count().Should().Be(books.Count);

        }

        [Fact]
        public async Task GetAll_WhenNoBooksExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<Book>();
            _unitOfWork.BookRepository.GetAllAsync().Returns(emptyList);
            _mapper.Map<IEnumerable<DisplayBookForMember>>(emptyList).Returns(new List<DisplayBookForMember>());

            // Act
            var result = await _bookService.GetAll();

            // Assert
            result.Should().BeEmpty();
        }
        #endregion

        #region AddBook Tests
        [Fact]
        public void AddBook_ValidDisplayBook_AddsBookToRepository()
        {
            // Arrange
            var displayBook = new DisplayBookForAdmin
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                ISBN = "0987654321",
                IsAvailable = true
            };
            var book = new Book
            {
                Title = displayBook.Title,
                Author = displayBook.Author,
                Description = displayBook.Description,
                ISBN = displayBook.ISBN,
                IsAvailable = displayBook.IsAvailable
            };
            _mapper.Map<Book>(displayBook).Returns(book);
            // Act
            _bookService.AddBook(displayBook);
            // Assert
            _unitOfWork.BookRepository.Received(1).Add(Arg.Is<Book>(b =>
                b.Title == displayBook.Title &&
                b.Author == displayBook.Author &&
                b.Description == displayBook.Description &&
                b.ISBN == displayBook.ISBN &&
                b.IsAvailable == displayBook.IsAvailable
            ));
        }
        #endregion

        #region UpddateBook Tests
        [Fact]
        public async Task UpdateBook_WithValidBook_ShouldUpdateInRepository()
        {
            // Arrange
            int bookId = 1;
            var existingBook = new Book { Id = bookId, Title = "Old Title" };

            var displayBook = new DisplayBookForAdmin
            {
                Title = "Updated Title",
                Author = "Updated Author",
                ISBN = "2222222222",
                IsAvailable = true
            };

            var updatedBook = new Book
            {
                Id = bookId,
                Title = displayBook.Title,
                Author = displayBook.Author,
                ISBN = displayBook.ISBN,
                IsAvailable = displayBook.IsAvailable
            };

            _unitOfWork.BookRepository.GetByIdAsync(bookId).Returns(existingBook);
            _mapper.Map<Book>(displayBook).Returns(updatedBook);

            // Act
            await _bookService.UpdateBook(displayBook, bookId);

            // Assert
            _unitOfWork.BookRepository.Received(1).Update(Arg.Is<Book>(b =>
                b.Id == bookId && b.Title == displayBook.Title));
            await _unitOfWork.Received(1).SaveAsync();
        }

        [Fact]
        public async Task UpdateBook_WithNonExistentBook_ShouldNotUpdate()
        {
            // Arrange
            int bookId = 999;
            var displayBook = new DisplayBookForAdmin { Title = "New Title" };

            _unitOfWork.BookRepository.GetByIdAsync(bookId)
                .Returns(Task.FromResult<Book>(null));

            // Act
            await _bookService.UpdateBook(displayBook, bookId);

            // Assert
            _unitOfWork.BookRepository.DidNotReceive().Update(Arg.Any<Book>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }

        #endregion

        #region DeleteBook Tests
        [Fact]
        public async Task DeleteBook_BookExists_DeletesBookAndReturnsMessage()
        {
            // Arrange
            int bookId = 1;
            _unitOfWork.BookRepository.IsExist(bookId).Returns(true);

            // Act
            var result = await _bookService.DeleteBook(bookId);

            // Assert
            result.Should().Be("Book Deleted Successfully");
            _unitOfWork.BookRepository.Received(1).Delete(bookId);
            _unitOfWork.Received(1).Save();
        }
        [Fact]
        public async Task DeleteBook_BookDoesNotExist_ReturnsNotFoundMessage()
        {
            // Arrange
            int bookId = 1;
            _unitOfWork.BookRepository.IsExist(bookId).Returns(false);
            // Act
            var result = await _bookService.DeleteBook(bookId);
            // Assert
            result.Should().Be("Book Not Found");
            _unitOfWork.BookRepository.DidNotReceive().Delete(bookId);
            _unitOfWork.DidNotReceive().Save();
        }
        #endregion

        #region GetAvailableBooks Tests
        [Fact]
        public async Task GetAvailableBooks_ReturnsOnlyAvailableBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", IsAvailable = true },
                new Book { Id = 2, Title = "Book 2", IsAvailable = false },
                new Book { Id = 3, Title = "Book 3", IsAvailable = true }
            };
            _unitOfWork.BookRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Book>)books));
            var availableBooks = books.Where(b =>(bool) b.IsAvailable).Select(b => new DisplayBookForMember
            {
                Title = b.Title
            }).ToList();

            _mapper.Map<DisplayBookForMember>(Arg.Is<Book>(b => b.Id == 1))
                .Returns(new DisplayBookForMember { Title = "Available Book" });
            _mapper.Map<DisplayBookForMember>(Arg.Is<Book>(b => b.Id == 3))
                .Returns(new DisplayBookForMember { Title = "Another Available" });
            // Act
            var result = await _bookService.GetAvailableBooks();
            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(2);
            result.All(b => b.Title == "Available Book" || b.Title == "Another Available").Should().BeTrue();
        }
        #endregion

        #region BorrowBook Tests

        [Fact]
        public async Task BorrowBook_WhenUserCanBorrowAndBookAvailable_ShouldSucceed()
        {
            // Arrange
            int userId = 1;
            var displayBook = new DisplayBookForMember { Title = "Borrowable Book" };
            var book = new Book
            {
                Id = 1,
                Title = displayBook.Title,
                IsAvailable = true
            };

            var bookLoan = new BookLoan
            {
                Id = 1,
                UserId = userId,
                BookId = book.Id,
                Status = (int)LoanStatus.Borrowed
            };

            _unitOfWork.BookLoanRepository.CanBorrow(userId).Returns(true);
            _unitOfWork.BookRepository.GetBookByTitle(displayBook.Title).Returns(book);
            _mapper.Map<BookLoan>(Arg.Any<AddBookLoanDTO>()).Returns(bookLoan);

            // Act
            var result = await _bookService.BorrowBook(displayBook);

            // Assert
            result.Loan.Should().NotBeNull();
            result.Message.Should().Be("Book has been borrowed successfully.");
            _unitOfWork.BookRepository.Received(1).Update(Arg.Is<Book>(b => (bool)!b.IsAvailable));
            await _unitOfWork.Received(1).SaveAsync();
        }

        [Fact]
        public async Task BorrowBook_WhenUserReachedBorrowingLimit_ShouldFail()
        {
            // Arrange
            int userId = 1;
            var displayBook = new DisplayBookForMember { Title = "Book" };

            _unitOfWork.BookLoanRepository.CanBorrow(userId).Returns(false);

            // Act
            var result = await _bookService.BorrowBook(displayBook);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("Cannot borrow the book. You  have reached the borrowing limit ");
        }

        [Fact]
        public async Task BorrowBook_WhenBookNotAvailable_ShouldFail()
        {
            // Arrange
            int userId = 1;
            var displayBook = new DisplayBookForMember { Title = "Unavailable Book" };
            var book = new Book
            {
                Id = 1,
                Title = displayBook.Title,
                IsAvailable = false
            };

            _unitOfWork.BookLoanRepository.CanBorrow(userId).Returns(true);
            _unitOfWork.BookRepository.GetBookByTitle(displayBook.Title).Returns(book);

            // Act
            var result = await _bookService.BorrowBook(displayBook);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("Cannot borrow the book , the book is not available.");
        }
        #endregion

        #region ReturnBook Tests
        [Fact]
        public async Task ReturnBook_WhenBookIsValid_ShouldReturnSuccessfully()
        {
            // Arrange
            int userId = 1;
            var displayBook = new DisplayBookForMember { Title = "Returned Book" };
            var book = new Book
            {
                Id = 1,
                Title = displayBook.Title,
                IsAvailable = false
            };

            var bookLoan = new BookLoan
            {
                Id = 1,
                UserId = userId,
                BookId = book.Id,
                Status = (int)LoanStatus.Borrowed
            };

            _unitOfWork.BookRepository.GetBookByTitle(displayBook.Title).Returns(book);
            _unitOfWork.BookLoanRepository.GetBookLoanRecord(userId, book.Id).Returns(bookLoan);

            // Act
            var result = await _bookService.ReturnBook(displayBook);

            // Assert
            result.Loan.Should().NotBeNull();
            result.Message.Should().Be("Book has been Returned successfully.");
            result.Loan.Status.Should().Be((int)LoanStatus.Returned);
            result.Loan.ReturnDate.Should().NotBeNull();

            _unitOfWork.BookRepository.Received(1).Update(Arg.Is<Book>(b => (bool) b.IsAvailable));
        }

        [Fact]
        public async Task ReturnBook_WhenBookAlreadyReturned_ShouldReturnAlreadyReturnedMessage()
        {
            // Arrange
            int userId = 1;
            var displayBook = new DisplayBookForMember { Title = "Already Returned Book" };
            var book = new Book
            {
                Id = 1,
                Title = displayBook.Title,
                IsAvailable = true
            };

            var bookLoan = new BookLoan
            {
                Id = 1,
                UserId = userId,
                BookId = book.Id,
                Status = (int)LoanStatus.Returned,
                ReturnDate = DateTime.Now.AddDays(-1)
            };

            _unitOfWork.BookRepository.GetBookByTitle(displayBook.Title).Returns(book);
            _unitOfWork.BookLoanRepository.GetBookLoanRecord(userId, book.Id).Returns(bookLoan);

            // Act
            var result = await _bookService.ReturnBook(displayBook);

            // Assert
            result.Loan.Should().BeNull();
            result.Message.Should().Be("This book has already been returned.");
        }
        #endregion

    }
}
