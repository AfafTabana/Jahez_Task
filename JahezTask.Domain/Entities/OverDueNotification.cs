using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JahezTask.Domain.Entities
{
    public class OverDueNotification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BookLoan")]
        public int BookLoanId { get; set; }
        public BookLoan BookLoan { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
        public DateTime NotificationDate { get; set; }

        public int NotificationType { get; set; }
        public string NotificationMessage { get; set; }

        public bool IsSent { get; set; }

        public DateTime SentAt { get; set; }
    }
}
