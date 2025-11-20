
using System.ComponentModel.DataAnnotations.Schema;

namespace JahezTask.Application.DTOs.BookLoan
{
    public class AddBookLoanDTO
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}
