using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace JahezTask.Application.DTOs.BookLoan.Commands.AddBook
{
    public class AddBookLoanCommand : IRequest<JahezTask.Domain.Entities.BookLoan>
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}
