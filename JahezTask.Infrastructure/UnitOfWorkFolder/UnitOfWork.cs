using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Persistence.Data;
using JahezTask.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace JahezTask.Infrastructure.UnitOfWorkFolder
{
    public class unitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        private IBookRepository _BookRepository;

        private IBookLoanRepository _BookLoanRepository;

        private INotificationRepository notificationRepository;

        private IDbContextTransaction _transaction;

        public unitOfWork( AppDbContext _context ) {

            context = _context;
            _BookRepository = new BookRepository(context);
            _BookLoanRepository = new BookLoanRepository(context);
            notificationRepository = new NotificationRepository(context);
            


        }

        public IBookRepository BookRepository { get {
                
                if (_BookRepository == null)
                {
                    _BookRepository = new BookRepository(context);
                }
                
                return _BookRepository; 
            
            } }


        public IBookLoanRepository BookLoanRepository
        {
            get { 
                if (_BookLoanRepository == null)
                {
                    _BookLoanRepository = new BookLoanRepository(context);
                }
                
                
                return _BookLoanRepository; 
            
            }
        }

        public INotificationRepository NotificationRepository
        {
            get
            {
                if (notificationRepository == null)
                {
                    notificationRepository = new NotificationRepository(context);
                }
                return notificationRepository;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken =default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to commit.");
            }

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync( cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to rollback.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }


        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
