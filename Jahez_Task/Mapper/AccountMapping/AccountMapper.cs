using AutoMapper;
using Jahez_Task.DTOs.Account;
using Jahez_Task.Models;

namespace Jahez_Task.Mapper.AccountMapping
{
    public class AccountMapper : Profile
    {
        public AccountMapper() { 
        
            CreateMap<ApplicationUser , RegisterDTO>().ReverseMap();    
        }
    }
}
