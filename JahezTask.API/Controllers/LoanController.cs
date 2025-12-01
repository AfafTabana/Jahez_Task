using JahezTask.Application.DTOs.Book;
using JahezTask.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        public readonly IBookService bookService;

        public LoanController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        //[Authorize(Roles = "member")]
        [HttpPost("BorrowBook")]

        public async Task<IActionResult> BorrowBook( [FromBody] DisplayBookForMember book , CancellationToken cancellationToken)
        {

            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
                var (Loan, message) = await bookService.BorrowBook(book , cancellationToken);
                if (Loan == null)
                    return BadRequest(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //[Authorize(Roles = "member")]
        [HttpPost("ReturnBook")]

        public async Task<IActionResult> ReturnBook( DisplayBookForMember book , CancellationToken cancellationToken)
        {
            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
                var (loan, message) = await bookService.ReturnBook(book , cancellationToken);
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

