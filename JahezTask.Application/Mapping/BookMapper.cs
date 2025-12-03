using AutoMapper;
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.DTOs.Book.Commands.DeleteBook;
using JahezTask.Application.DTOs.Book.Commands.UpdateBook;
using JahezTask.Application.DTOs.Book.Queries.GetAvailableBooks;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetail;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Mapping

{
    public class BookMapper : Profile
    {
        public BookMapper() {
            CreateMap<Book , DisplayBookForAdmin>().ReverseMap();
            CreateMap<Book , DisplayBookForMember>().ReverseMap();
            CreateMap<Book , GetBookDetailDTO>().ReverseMap();
            CreateMap<Book , GetAvailableBooksDto>().ReverseMap();
            CreateMap<Book , CreateBookCommand>().ReverseMap();
            CreateMap<Book , DeleteCommand>().ReverseMap();
            CreateMap<Book , UpdateBookCommand>().ReverseMap();


        }

    }
}
