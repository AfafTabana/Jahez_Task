using AutoMapper;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember
{
    public class GetBookDetailForMemberHandler : IRequestHandler<GetBookDetailForMemberQuery, IEnumerable<DisplayBookForMember>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public GetBookDetailForMemberHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DisplayBookForMember>> Handle(GetBookDetailForMemberQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetAllAsync(cancellationToken);
            var mappedBook = _mapper.Map<IEnumerable<DisplayBookForMember>>(book);
            return mappedBook;
        }
    }
}
