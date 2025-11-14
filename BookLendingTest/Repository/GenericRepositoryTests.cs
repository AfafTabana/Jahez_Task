using FluentAssertions;
using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Repository
{
    public class GenericRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly GenericRepository<Book> _repository;

        public GenericRepositoryTests()
        {
            // Create unique in-memory database for each test instance
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _repository = new GenericRepository<Book>(_context);
        }

        [Fact]
        public async Task GetAllAsync_WithData_ShouldReturnAllEntities()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "111", IsAvailable = true , Description ="Book1Desc" },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", ISBN = "222", IsAvailable = false , Description ="Book2Desc"},
                new Book { Id = 3, Title = "Book 3", Author = "Author 3", ISBN = "333", IsAvailable = true, Description ="Book3Desc" }
            };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(books);
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "111", IsAvailable = true ,Description = "Book1Desc" };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            // Act
            var result = await _repository.GetByIdAsync(book.Id);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Add_ShouldAddEntityToDatabase()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "New Book", Author = "New Author", ISBN = "123", IsAvailable = true ,  Description = "BookDesc" };
            // Act
            _repository.Add(book);
            await _context.SaveChangesAsync();
            // Assert
            var addedBook = await _context.Books.FindAsync(book.Id);
            addedBook.Should().NotBeNull();

            addedBook.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task Delete_WithExistingId_ShouldRemoveEntityFromDatabase()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book to Delete", Author = "Author", ISBN = "123", IsAvailable = true ,  Description = "BookDesc" };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            // Act
            _repository.Delete(book.Id);
            await _context.SaveChangesAsync();
            // Assert
            var deletedBook = await _context.Books.FindAsync(book.Id);
            deletedBook.Should().BeNull();
        }
        [Fact]
        public async Task Delete_WithNonExistingId_ShouldNotThrowException()
        {
            // Act
            Action act = () => _repository.Delete(999);
            // Assert
            act.Should().NotThrow();
        }
        [Fact]
        public async Task Update_WithExistingEntity_ShouldUpdateEntityInDatabase()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Original Title", Author = "Original Author", ISBN = "123", IsAvailable = true, Description = "BookDesc" };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            // Modify the book
            book.Title = "Updated Title";
            book.Author = "Updated Author";
            // Act
            _repository.Update(book);
            await _context.SaveChangesAsync();
            // Assert
            var updatedBook = await _context.Books.FindAsync(book.Id);
            updatedBook.Should().NotBeNull();
            updatedBook.Title.Should().Be("Updated Title");
            updatedBook.Author.Should().Be("Updated Author");
        }

        [Fact]
        public async Task Update_WithNonExistingEntity_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var book = new Book { Id = 999, Title = "Non-existing Book", Author = "Author", ISBN = "123", IsAvailable = true, Description = "BookDesc" };
            // Act
            Action act = () => _repository.Update(book);
            // Assert
            act.Should().Throw<KeyNotFoundException>()
                .WithMessage("Entity of type Book with ID 999 not found.");
        }

        [Fact]
        public async Task IsExist_WithExistingId_ShouldReturnTrue()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Existing Book",
                Author = "Author",
                ISBN = "123456",
                IsAvailable = true ,
                Description = "Book1Desc"
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsExist(book.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsExist_WithNonExistingId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.IsExist(999);
            // Assert
            result.Should().BeFalse();
        }
    }
}
