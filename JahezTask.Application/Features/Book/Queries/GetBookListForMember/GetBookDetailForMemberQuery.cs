using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember
{
    public class GetBookDetailForMemberQuery : IRequest<IEnumerable<DisplayBookForMember>>
    {
    }
}
