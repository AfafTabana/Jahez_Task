using AutoMapper;
using Jahez_Task.Models;

namespace Jahez_Task.Mapper.BookLoanMapping
{
    public class BookLoanMapper : Profile
    {
        public BookLoanMapper() {
          CreateMap<BookLoan , BookLoanMapper>().ReverseMap();
        }
    }
}
