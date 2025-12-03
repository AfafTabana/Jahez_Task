using FluentAssertions;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using JahezTask.Persistence.Data;
using JahezTask.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookLendingTest.Repository
{
    public class BookLoanRepositoryTests 
    {
        private readonly AppDbContext _context;
        private readonly BookLoanRepository _repository;

        public BookLoanRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _repository = new BookLoanRepository(_context);
        }

        [Fact]
        public async Task GetBookLoanByUserId_WithExistingUser_ShouldReturnLoans()
        {
            // Arrange
            var loans = new List<BookLoan>
            {
                new BookLoan { Id = 1, UserId = 1, BookId = 10, Status = (int)LoanStatus.Borrowed },
                new BookLoan { Id = 2, UserId = 1, BookId = 20, Status = (int)LoanStatus.Returned },
                new BookLoan { Id = 3, UserId = 2, BookId = 30, Status = (int)LoanStatus.Borrowed }
            };

            await _context.BookLoans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBookLoanByUserId(1, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.UserId == 1);
        }

        [Fact]
        public async Task GetBookLoanByUserId_WithNoLoans_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetBookLoanByUserId(999, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CanBorrow_WhenAllLoansReturned_ShouldReturnTrue()
        {
            // Arrange
            var loans = new List<BookLoan>
            {
                new BookLoan { Id = 1, UserId = 1, BookId = 10, Status = (int)LoanStatus.Returned },
                new BookLoan { Id = 2, UserId = 1, BookId = 20, Status = (int)LoanStatus.Returned }
            };

            await _context.BookLoans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.CanBorrow(1, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanBorrow_WhenAtLeastOneLoanNotReturned_ShouldReturnFalse()
        {
            // Arrange
            var loans = new List<BookLoan>
            {
                new BookLoan { Id = 1, UserId = 1, BookId = 10, Status = (int)LoanStatus.Borrowed },
                new BookLoan { Id = 2, UserId = 1, BookId = 20, Status = (int)LoanStatus.Returned }
            };

            await _context.BookLoans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.CanBorrow(1, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CanBorrow_WhenUserHasNoLoans_ShouldReturnTrue()
        {
            // Act
            var result = await _repository.CanBorrow(999, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetBookLoanRecord_WithExistingRecord_ShouldReturnLoan()
        {
            // Arrange
            var loan = new BookLoan
            {
                Id = 1,
                UserId = 5,
                BookId = 50,
                Status = (int)LoanStatus.Borrowed
            };

            await _context.BookLoans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBookLoanRecord(5, 50, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(5);
            result.BookId.Should().Be(50);
            result.Status.Should().Be((int)LoanStatus.Borrowed);
        }

        [Fact]
        public async Task GetBookLoanRecord_WithNonExistingRecord_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetBookLoanRecord(9, 99, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBookLoanRecord_WithMultipleRecords_ShouldReturnCorrectOne()
        {
            // Arrange
            var loans = new List<BookLoan>
            {
                new BookLoan { Id = 1, UserId = 1, BookId = 10, Status = (int)LoanStatus.Borrowed },
                new BookLoan { Id = 2, UserId = 1, BookId = 20, Status = (int)LoanStatus.Returned },
                new BookLoan { Id = 3, UserId = 2, BookId = 10, Status = (int)LoanStatus.Borrowed }
            };

            await _context.BookLoans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBookLoanRecord(1, 20, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(2);
            result.UserId.Should().Be(1);
            result.BookId.Should().Be(20);
        }

     
    }
}