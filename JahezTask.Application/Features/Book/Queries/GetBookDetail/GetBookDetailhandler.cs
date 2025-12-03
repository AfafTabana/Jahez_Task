using AutoMapper;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetBookDetail
{
    public class GetBookDetailhandler : IRequestHandler<GetBookDetailQuery, GetBookDetailDTO>
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper _mapper;
        public GetBookDetailhandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<GetBookDetailDTO> Handle(GetBookDetailQuery request, CancellationToken cancellationToken)
        {
            if (!await _bookRepository.IsExist(request.BookId, cancellationToken))
            {
                return null;
            }
           var book =  await _bookRepository.GetByIdAsync(request.BookId , cancellationToken);
           var mappedBook = _mapper.Map<GetBookDetailDTO>(book);
           return mappedBook;
        }
    }
}
