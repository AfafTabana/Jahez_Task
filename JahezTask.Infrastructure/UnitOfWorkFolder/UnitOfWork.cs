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
        private readonly AppDbContext Context;

        private IBookRepository _BookRepository;

        private IBookLoanRepository _BookLoanRepository;

        private INotificationRepository notificationRepository;

        private IDbContextTransaction _transaction;

        public unitOfWork( AppDbContext _context ) {

            Context = _context;
            _BookRepository = new BookRepository(Context);
            _BookLoanRepository = new BookLoanRepository(Context);
            notificationRepository = new NotificationRepository(Context);
            


        }

        public IBookRepository BookRepository { get {
                
                if (_BookRepository == null)
                {
                    _BookRepository = new BookRepository(Context);
                }
                
                return _BookRepository; 
            
            } }


        public IBookLoanRepository BookLoanRepository
        {
            get { 
                if (_BookLoanRepository == null)
                {
                    _BookLoanRepository = new BookLoanRepository(Context);
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
                    notificationRepository = new NotificationRepository(Context);
                }
                return notificationRepository;
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to commit.");
            }

            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
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
            Context.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
