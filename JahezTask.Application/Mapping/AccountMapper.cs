using AutoMapper;
using JahezTask.Application.Features.Account.Commands.Register;
using JahezTask.Domain.Entities;

namespace JahezTask.Application.Mapping
{
    public class AccountMapper : Profile
    {
        public AccountMapper() { 
        
            CreateMap<ApplicationUser , RegisterCommand>().ReverseMap();    
        }
    }
}
