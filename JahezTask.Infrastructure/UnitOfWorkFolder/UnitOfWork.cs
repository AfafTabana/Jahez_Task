using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Persistence.Data;
using JahezTask.Persistence.Repositories;

namespace JahezTask.Infrastructure.UnitOfWorkFolder
{
    public class unitOfWork : IUnitOfWork
    {
        private readonly AppDbContext Context;

        private IBookRepository _BookRepository;

        private IBookLoanRepository _BookLoanRepository;

        private INotificationRepository notificationRepository;

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

        public void Save()
        {
            Context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
