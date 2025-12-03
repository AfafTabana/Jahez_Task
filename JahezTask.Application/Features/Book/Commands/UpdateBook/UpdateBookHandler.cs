using AutoMapper;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Commands.UpdateBook
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand>
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper _mapper;
        public UpdateBookHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var existing = await _bookRepository.GetByIdAsync( request.Id, cancellationToken);
            if (request != null && existing != null) {

                JahezTask.Domain.Entities.Book book = _mapper.Map<JahezTask.Domain.Entities.Book>(request);

                _bookRepository.Update(book);
                await _bookRepository.SaveAsync(cancellationToken);

            }
              

          
        }
    }
}
