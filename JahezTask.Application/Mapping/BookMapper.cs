using AutoMapper;
using JahezTask.Application.DTOs.Book;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Mapping

{
    public class BookMapper : Profile
    {
        public BookMapper() {
            CreateMap<Book , DisplayBookForAdmin>().ReverseMap();
            CreateMap<Book , DisplayBookForMember>().ReverseMap();
        
        
        }

    }
}
