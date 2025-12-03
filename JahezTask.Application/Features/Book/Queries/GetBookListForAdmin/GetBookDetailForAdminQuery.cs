using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin
{
    public class GetBookDetailForAdminQuery : IRequest<IEnumerable<DisplayBookForAdmin>> 
    {
    }
}
