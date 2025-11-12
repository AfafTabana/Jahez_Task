using AutoMapper;
using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.Models;

namespace Jahez_Task.Mapper.BookMapping

{
    public class BookMapper : Profile
    {
        public BookMapper() {
            CreateMap<Book , DisplayBook>().ReverseMap();
            CreateMap<Book , DIsplayBook>().ReverseMap();
        
        
        }

    }
}
