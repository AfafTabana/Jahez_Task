using Jahez_Task.Models;
using Jahez_Task.Repository.BookLoanRepo;
using Jahez_Task.Repository.BookRepo;

namespace Jahez_Task.UnitOfWork
{
    public class unitOfWork
    {
        public readonly AppDbContext Context;

        public IBookRepository _BookRepository;

        public IBookLoanRepository _BookLoanRepository;

        public unitOfWork( AppDbContext _context ) {

            Context = _context;
            _BookRepository = new BookRepository(Context);
            _BookLoanRepository = new BookLoanRepository(Context);
          
       
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

        public void Save()
        {
            Context.SaveChanges();
        }

        public void SaveAsync()
        {
            Context.SaveChangesAsync();
        }
    }
}
