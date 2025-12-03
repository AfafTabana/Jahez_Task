using AutoMapper;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Queries.GetAvailableBooks
{
    public class GetAvailableBooksHandler : IRequestHandler<GetAvailableBooksQuery, List<GetAvailableBooksDto>>
    {
        private IBookRepository bookRepository;

        private IMapper _mapper;

        public GetAvailableBooksHandler(IBookRepository bookRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAvailableBooksDto>> Handle(GetAvailableBooksQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<JahezTask.Domain.Entities.Book> AllBooks = await bookRepository.GetAllAsync(cancellationToken);
            List<GetAvailableBooksDto> _AllBooks = new List<GetAvailableBooksDto>();
            foreach (var book in AllBooks)
            {
                if (book.IsAvailable == true)
                {
                    _AllBooks.Add(_mapper.Map<GetAvailableBooksDto>(book));

                }
            }

            return _AllBooks;

        }
    }
}
