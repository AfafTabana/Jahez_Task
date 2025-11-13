using AutoMapper;
using Jahez_Task.DTOs.BookLoan;
using Jahez_Task.Models;

namespace Jahez_Task.Mapper.BookLoanMapping
{
    public class BookLoanMapper : Profile
    {
        public BookLoanMapper() {
          
          CreateMap<BookLoan , AddBookLoanDTO>().ReverseMap();
        }
    }
}
