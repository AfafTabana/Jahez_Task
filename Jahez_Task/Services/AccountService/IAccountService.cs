using Jahez_Task.DTOs.Account;

namespace Jahez_Task.Services.AccountService
{
    public interface IAccountService
    {
        Task<(bool Success, string Message)> Register(RegisterDTO registerDTO);
        Task<(bool Success, string Message)> Login (LoginDTO loginDTO);

    }
}
