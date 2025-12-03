namespace JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin
{
    public class DisplayBookForAdmin
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}
