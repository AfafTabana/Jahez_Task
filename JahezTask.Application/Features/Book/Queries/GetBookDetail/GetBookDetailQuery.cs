using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookDetail
{
    public class GetBookDetailQuery : IRequest<GetBookDetailDTO>
    {
        public int BookId { get; set; }
    }
}
