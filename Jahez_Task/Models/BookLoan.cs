using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jahez_Task.Models
{
    public class BookLoan
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; } 
        
        public virtual List<OverDueNotification> OverDueNotifications { get; set; }
    }
}
