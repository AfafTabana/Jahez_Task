using AutoMapper;
using JahezTask.Application.DTOs.BookLoan.Commands.AddBook;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Mapping
{
    public class BookLoanMapper : Profile
    {
        public BookLoanMapper() {
          
          CreateMap<BookLoan , AddBookLoanCommand>().ReverseMap();

        }
    }
}
