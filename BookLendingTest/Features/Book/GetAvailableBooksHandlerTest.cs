using AutoMapper;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Commands.DeleteBook;
using JahezTask.Application.DTOs.Book.Queries.GetAvailableBooks;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Features.Book
{
    public class GetAvailableBooksHandlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly GetAvailableBooksHandler _handler;
        public GetAvailableBooksHandlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new GetAvailableBooksHandler(_bookRepository, _mapper);

        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyAvailableBooks()
        {
            // Arrange
            var books = new List<JahezTask.Domain.Entities.Book>
            {
                new JahezTask.Domain.Entities.Book { Id = 1, Title = "Book 1", IsAvailable = true },
                new JahezTask.Domain.Entities.Book { Id = 2, Title = "Book 2", IsAvailable = false },
                new JahezTask.Domain.Entities.Book { Id = 3, Title = "Book 3", IsAvailable = true }
            };

            _bookRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(books);

            var displayBook1 = new GetAvailableBooksDto { Title = "Available Book 1" };
            var displayBook3 = new GetAvailableBooksDto { Title = "Available Book 3" };

            _mapper.Map<GetAvailableBooksDto>(Arg.Is<JahezTask.Domain.Entities.Book>(b => b.Id == 1)).Returns(displayBook1);
            _mapper.Map<GetAvailableBooksDto>(Arg.Is<JahezTask.Domain.Entities.Book>(b => b.Id == 3)).Returns(displayBook3);
            var query = new GetAvailableBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);


            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(2);
            result.Select(b => b.Title).Should().Contain(new[] { "Available Book 1", "Available Book 3" });
        }
    }
}
