
using Jahez_Task.Services.AccountService;
using JahezTask.Application.DTOs.Account;
using JahezTask.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

       private readonly IAccountService accountService;

       public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(RegisterDTO registerDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success , Message) = await accountService.Register(registerDTO , cancellationToken);
            if(!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO , CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success, Message) = await accountService.Login(loginDTO , cancellationToken);
            if (!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }



    }
}
