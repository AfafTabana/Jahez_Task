using FluentAssertions;
using JahezTask.Domain.Entities;
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
    

      
    }
}

