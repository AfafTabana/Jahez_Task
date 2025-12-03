using AutoMapper;
using JahezTask.Application.DTOs.BookLoan.Commands.AddBook;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Features.BookLoan.Commands.AddBookLoan
{
    public class AddBookLoanCommandHandler : IRequestHandler<AddBookLoanCommand, JahezTask.Domain.Entities.BookLoan>
    {
        private readonly IBookLoanRepository _bookLoanRepository; 

        private readonly IMapper mapper;
        public AddBookLoanCommandHandler(IBookLoanRepository bookLoanRepository, IMapper mapper)
        {
            _bookLoanRepository = bookLoanRepository;
            this.mapper = mapper;
        }

        public async Task<Domain.Entities.BookLoan> Handle(AddBookLoanCommand request, CancellationToken cancellationToken)
        {
           
               JahezTask.Domain.Entities.BookLoan bookLoan = mapper.Map<Domain.Entities.BookLoan>(request);
               _bookLoanRepository.Add(bookLoan);
               await _bookLoanRepository.SaveAsync(cancellationToken);


            return bookLoan;
        }
    }
}
