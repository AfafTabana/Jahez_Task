using MediatR;

namespace JahezTask.Application.Features.Account.Commands.Login
{
    public class LoginCommand : IRequest<(bool Success, string Message)>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
