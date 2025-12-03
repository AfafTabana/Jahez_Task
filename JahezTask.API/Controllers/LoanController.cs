using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.Features.BookLoan.Commands.BorrowBook;
using JahezTask.Application.Features.BookLoan.Commands.ReturnBook;
using JahezTask.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        public readonly IMediator mediator;

        public LoanController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "member")]
        [HttpPost("BorrowBook")]

        public async Task<IActionResult> BorrowBook( [FromBody] BorrowBookCommand command , CancellationToken cancellationToken)
        {

            if (command == null)
                return BadRequest("Book data is required.");

            try
            {
                var (Loan, message) = await mediator.Send(command , cancellationToken);
                if (Loan == null)
                    return BadRequest(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "member")]
        [HttpPost("ReturnBook")]

        public async Task<IActionResult> ReturnBook( ReturnBookCommand command , CancellationToken cancellationToken)
        {
            if (command == null)
                return BadRequest("Book data is required.");

            try
            {
                var (loan, message) = await mediator.Send(command , cancellationToken);
                if (loan == null)
                    return BadRequest(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

