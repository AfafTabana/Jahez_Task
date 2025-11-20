using JahezTask.Application.DTOs.Account;

namespace JahezTask.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<(bool Success, string Message)> Register(RegisterDTO registerDTO);
        Task<(bool Success, string Message)> Login (LoginDTO loginDTO);

    }
}
