using Jahez_Task.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jahez_Task.DTOs.BookLoan
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
