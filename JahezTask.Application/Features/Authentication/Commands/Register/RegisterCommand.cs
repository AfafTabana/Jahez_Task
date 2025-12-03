using MediatR;

namespace JahezTask.Application.Features.Account.Commands.Register
{
    public class RegisterCommand : IRequest<(bool Success, string Message)>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
