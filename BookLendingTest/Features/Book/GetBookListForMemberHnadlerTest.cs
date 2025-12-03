using AutoMapper;
using FluentAssertions;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetail;
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
    public class GetBookListForMemberHnadlerTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly GetBookDetailForMemberHandler _handler;
        public GetBookListForMemberHnadlerTest()
        {
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IBookRepository>();
            _handler = new GetBookDetailForMemberHandler(_bookRepository, _mapper);

        }

       
        [Fact]
        public async Task Handle_ShouldReturnMappedBooks_WhenBooksExist()
        {
            // Arrange 
            var books = new List<JahezTask.Domain.Entities.Book>
            {
                new JahezTask.Domain.Entities.Book { Id = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1" },
                new JahezTask.Domain.Entities.Book { Id = 2, Title = "Book 2", Author = "Author 2", Description = "Description 2" }
            };

            _bookRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(books);
            var displayBooks = books.Select(b => new DisplayBookForMember
            {
                Title = b.Title,
                Author = b.Author,
                Description = b.Description
            }).ToList();

            _mapper.Map<IEnumerable<DisplayBookForMember>>(books).Returns(displayBooks);

            //query
            var query = new GetBookDetailForMemberQuery ();
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenBooksNotExist()
        {
            // Arrange
            var emptyList = new List<JahezTask.Domain.Entities.Book>();
            _bookRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(emptyList);
            _mapper.Map<IEnumerable<DisplayBookForMember>>(emptyList).Returns(new List<DisplayBookForMember>());

            //query
            var query = new GetBookDetailForMemberQuery();
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    

    }
}
