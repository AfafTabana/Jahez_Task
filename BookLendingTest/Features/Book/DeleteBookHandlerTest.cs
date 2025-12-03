using AutoMapper;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Commands.DeleteBook;
using JahezTask.Application.DTOs.Book.Commands.UpdateBook;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Features.Book
{
    public class DeleteBookHandlerTest
    {

        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly DeleteCommandHandler _handler;
        public DeleteBookHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new DeleteCommandHandler(_bookRepository, _mapper);

        }

        [Fact]
        public async Task Handle_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange

            int bookId = 1;
            var book = new JahezTask.Domain.Entities.Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                Description = "Test Description",
                IsAvailable = true
            };
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

            var command = new DeleteCommand { BookId = bookId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.Should().Be("Book Deleted Successfully");
            _bookRepository.Received(1).Delete(bookId);
            await _bookRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            int bookId = 1;
            _bookRepository.IsExist(bookId, Arg.Any<CancellationToken>()).Returns(false);

            var command = new DeleteCommand { BookId = bookId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be("Book Not Found");
            _bookRepository.DidNotReceive().Delete(bookId);
            await _bookRepository.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
