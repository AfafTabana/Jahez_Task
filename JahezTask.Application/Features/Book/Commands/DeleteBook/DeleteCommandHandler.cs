using AutoMapper;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Commands.DeleteBook
{
    public class DeleteCommandHandler : IRequestHandler<DeleteCommand , string>
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper _mapper;
        public DeleteCommandHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<string> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            string Message = "";
          
            JahezTask.Domain.Entities.Book book = await _bookRepository.GetByIdAsync(request.BookId);
            if (book != null)
            {
                _bookRepository.Delete(request.BookId);
                await _bookRepository.SaveAsync(cancellationToken);
                Message = "Book Deleted Successfully";
                return Message;
            }else
            {
                Message = "Book Not Found";
                return Message;

            }
           
           
        }
    }
}
