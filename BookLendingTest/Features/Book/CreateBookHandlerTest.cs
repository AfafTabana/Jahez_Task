using AutoMapper;
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin;
using JahezTask.Application.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookLendingTest.Features.Book
{
    public class CreateBookHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly CreateBookCommandHandler _handler;
        public CreateBookHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new CreateBookCommandHandler(_bookRepository, _mapper);

        }
        [Fact]
        public async Task Handle_ShouldCreateBook_WhenValidCommand()
        {
            // Arrange
            var displayBook = new CreateBookCommand
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                ISBN = "0987654321",
                IsAvailable = true
            };

            var book = new JahezTask.Domain.Entities.Book
            {
                Title = displayBook.Title,
                Author = displayBook.Author,
                Description = displayBook.Description,
                ISBN = displayBook.ISBN,
                IsAvailable = displayBook.IsAvailable
            };

            _mapper.Map<JahezTask.Domain.Entities.Book>(displayBook).Returns(book);

            //query
            var query = new CreateBookCommand();
            // Act
            await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            _bookRepository.Received(1).Add(Arg.Is<JahezTask.Domain.Entities.Book>(b =>
               b.Title == displayBook.Title &&
               b.Author == displayBook.Author &&
               b.Description == displayBook.Description &&
               b.ISBN == displayBook.ISBN &&
               b.IsAvailable == displayBook.IsAvailable
           ));
            await _bookRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
