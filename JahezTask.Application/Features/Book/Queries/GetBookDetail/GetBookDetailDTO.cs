using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookDetail
{
    public class GetBookDetailDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}
