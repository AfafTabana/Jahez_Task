using Jahez_Task.DTOs.Account;
using Jahez_Task.Models;
using Jahez_Task.Services.AccountService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jahez_Task.Controllers
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

        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success , Message) = await accountService.Register(registerDTO);
            if(!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (Success, Message) = await accountService.Login(loginDTO);
            if (!Success)
                return BadRequest(new { Message });
            return Ok(new { Message });
        }



    }
}
