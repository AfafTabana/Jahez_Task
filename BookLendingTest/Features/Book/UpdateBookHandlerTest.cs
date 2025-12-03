using AutoMapper;
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.DTOs.Book.Commands.UpdateBook;
using JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin;
using JahezTask.Application.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Features.Book
{
    public class UpdateBookHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly UpdateBookHandler _handler;
        public UpdateBookHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new UpdateBookHandler(_bookRepository, _mapper);

        }
        [Fact]
        public async Task Handle_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
            int bookId = 1;
            var existingBook = new JahezTask.Domain.Entities.Book { Id = bookId, Title = "Old Title" };
            var displayBook = new UpdateBookCommand
            {
                Id = bookId ,
                Title = "Updated Title",
                Author = "Updated Author",
                ISBN = "2222222222",
                IsAvailable = true
            };
            var updatedBook = new JahezTask.Domain.Entities.Book
            {
                Id = bookId,
                Title = displayBook.Title,
                Author = displayBook.Author,
                ISBN = displayBook.ISBN,
                IsAvailable = displayBook.IsAvailable
            };

            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(existingBook);
            _mapper.Map<JahezTask.Domain.Entities.Book>(displayBook).Returns(updatedBook);

            // Act
            await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            _bookRepository.Received(1).Update(Arg.Is<JahezTask.Domain.Entities.Book>(b =>
                b.Id == bookId && b.Title == displayBook.Title));
            await _bookRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldNotUpdate_WhenBookDoesNotExist()
        {
            // Arrange
            int bookId = 999;
            var displayBook = new UpdateBookCommand { Title = "New Title" };

            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<JahezTask.Domain.Entities.Book>(null));

            // Act
            await _handler.Handle(displayBook, CancellationToken.None);

            // Assert
            _bookRepository.DidNotReceive().Update(Arg.Any<JahezTask.Domain.Entities.Book>());
            await _bookRepository.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
