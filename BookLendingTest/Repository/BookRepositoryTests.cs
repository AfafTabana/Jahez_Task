using FluentAssertions;
using Jahez_Task.Models;
using Jahez_Task.Repository.BookRepo;
using Jahez_Task.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Repository
{
    public class BookRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _repository = new BookRepository(_context);
        }
        [Fact]
        public async Task GetBookByTitle_WithExistingTitle_ShouldReturnBook()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Author X",
                ISBN = "12345",
                Description = "A test description",
                IsAvailable = true
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            // Act
            var result = _repository.GetBookByTitle("Test Book");

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Book");
            result.Should().BeEquivalentTo(book);
        }

        [Fact]
        public void GetBookByTitle_WithNonExistingTitle_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetBookByTitle("Does Not Exist");

            // Assert
            result.Should().BeNull();
        }
    }
}

