

using JahezTask.Application.Features.Account.Commands.Login;
using JahezTask.Application.Features.Account.Commands.Register;
using JahezTask.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IMediator mediator;

        public AccountController(IMediator mediator)
        {
           this.mediator = mediator;
        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success , Message) = await mediator.Send(command , cancellationToken);
            if(!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginCommand command , CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success, Message) = await mediator.Send(command , cancellationToken);
            if (!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }



    }
}
