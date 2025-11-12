using System.ComponentModel.DataAnnotations;

namespace Jahez_Task.Models
{
    public class Book
    {

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public bool? IsAvailable { get; set; } = true;

        public virtual List<BookLoan> BookLoans { get; set; }

    }
}
