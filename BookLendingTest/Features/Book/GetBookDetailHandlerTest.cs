using AutoMapper;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetail;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Features.Book
{
    public class GetBookDetailHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly GetBookDetailhandler _handler;
        public GetBookDetailHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new GetBookDetailhandler(_bookRepository, _mapper);

        }

        [Fact]
        public async Task Handle_ShouldReturnBookDetail_WhenBookExists()
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
           
            var displayBook = new GetBookDetailDTO
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                IsAvailable = (bool)book.IsAvailable,
                ISBN = book.ISBN
            };

            _bookRepository.IsExist(bookId, Arg.Any<CancellationToken>()).Returns(true);
            _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);
            _mapper.Map<GetBookDetailDTO>(book).Returns(displayBook);

            //query
            var query = new GetBookDetailQuery { BookId = bookId };
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Book");

        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenBookNotExists()
        {
            // Arrange
            int bookId = 1;
           _bookRepository.IsExist(bookId, Arg.Any<CancellationToken>()).Returns(false);
            //query
            var query = new GetBookDetailQuery {};
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }


    }
}
