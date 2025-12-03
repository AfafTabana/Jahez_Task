using AutoMapper;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Commands.AddBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand>
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper mapper;
        public CreateBookCommandHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            this.mapper = mapper;
        }
        public async Task Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request != null) {
                var book = mapper.Map<JahezTask.Domain.Entities.Book>(request);
                CreateCommandValidator validator = new CreateCommandValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (validationResult.Errors.Any())
                {
                    throw new Exception("Invalid Book ");
                }
                _bookRepository.Add(book);
                await _bookRepository.SaveAsync(cancellationToken);
            }

        }
    }
}
