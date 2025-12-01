using JahezTask.Application.DTOs.Account;

namespace JahezTask.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<(bool Success, string Message)> Register(RegisterDTO registerDTO , CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> Login (LoginDTO loginDTO , CancellationToken cancellationToken = default);

    }
}
