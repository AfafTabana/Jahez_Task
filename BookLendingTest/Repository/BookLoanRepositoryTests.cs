using FluentAssertions;
using JahezTask.Domain.Entities;
using JahezTask.Domain.Enums;
using JahezTask.Persistence.Data;
using JahezTask.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var result = _repository.GetBookLoanByUserId(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.UserId == 1);
        }

        [Fact]
        public void GetBookLoanByUserId_WithNoLoans_ShouldReturnEmptyList()
        {
            // Act
            var result = _repository.GetBookLoanByUserId(999);

            // Assert
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
            var result = _repository.CanBorrow(1);

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
            var result = _repository.CanBorrow(1);

            // Assert
            result.Should().BeFalse();
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
            var result = _repository.GetBookLoanRecord(5, 50);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(5);
            result.BookId.Should().Be(50);
        }

        [Fact]
        public void GetBookLoanRecord_WithNonExistingRecord_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetBookLoanRecord(9, 99);

            // Assert
            result.Should().BeNull();
        }
    }
}
