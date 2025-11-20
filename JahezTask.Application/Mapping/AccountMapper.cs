using AutoMapper;
using JahezTask.Application.DTOs.Account;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Mapping
{
    public class AccountMapper : Profile
    {
        public AccountMapper() { 
        
            CreateMap<ApplicationUser , RegisterDTO>().ReverseMap();    
        }
    }
}
