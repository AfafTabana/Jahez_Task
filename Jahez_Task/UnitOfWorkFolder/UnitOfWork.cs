using Jahez_Task.Models;
using Jahez_Task.Repository.BookLoanRepo;
using Jahez_Task.Repository.BookRepo;
using Jahez_Task.Repository.NotificationRepo;
using Jahez_Task.UnitOfWorkFolder;

namespace Jahez_Task.UnitOfWork
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
