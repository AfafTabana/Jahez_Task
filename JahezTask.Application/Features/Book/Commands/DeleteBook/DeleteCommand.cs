using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Commands.DeleteBook
{
    public class DeleteCommand : IRequest<string>
    {
        public int BookId { get; set; }
    }
}
