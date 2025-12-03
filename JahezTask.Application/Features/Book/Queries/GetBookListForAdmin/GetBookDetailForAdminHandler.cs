using AutoMapper;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin
{


    public class GetBookDetailForAdminHandler : IRequestHandler<GetBookDetailForAdminQuery, IEnumerable<DisplayBookForAdmin>>
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper _mapper;
        public GetBookDetailForAdminHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async  Task<IEnumerable<DisplayBookForAdmin>> Handle(GetBookDetailForAdminQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetAllAsync(cancellationToken);

            var mappedBook = _mapper.Map<IEnumerable<DisplayBookForAdmin>>(book);
            return mappedBook;
        }
    }
}
