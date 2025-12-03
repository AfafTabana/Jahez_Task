
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.DTOs.Book.Commands.DeleteBook;
using JahezTask.Application.DTOs.Book.Commands.UpdateBook;
using JahezTask.Application.DTOs.Book.Queries.GetAvailableBooks;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetail;
using JahezTask.Application.DTOs.Book.Queries.GetBookDetailForMember;
using JahezTask.Application.DTOs.Book.Queries.GetBookListForAdmin;
using JahezTask.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public readonly IMediator mediator;

        public BookController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [Authorize(Roles = "member")]
        [HttpGet("DisplayBooksForMembers")]
        public async Task<IActionResult> GetAllBooksForMembers(CancellationToken cancellationToken)
        {

            IEnumerable<DisplayBookForMember> Books = await mediator.Send(new GetBookDetailForMemberQuery() , cancellationToken);
            return Ok(Books);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("DisplayBooksForAdmin")]

        public async Task<IActionResult> GetAllBooksForAdmin(CancellationToken cancellationToken)
        {
            IEnumerable<DisplayBookForAdmin> Books = await mediator.Send(new GetBookDetailForAdminQuery() , cancellationToken);
            return Ok(Books);


        }
        [Authorize(Roles = "admin")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetBookById(int id , CancellationToken cancellationToken)
        {
            var Book = await mediator.Send(new GetBookDetailQuery() { BookId = id} , cancellationToken);
            return Ok(Book);

        }
        [Authorize(Roles = "admin")]
        [HttpPost("AddBook")]

        public async Task<IActionResult> AddBook(CreateBookCommand book , CancellationToken cancellationToken)
        {
            await mediator.Send(book , cancellationToken);
            return Ok("Book Added Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpPut("UpdateBook")]

        public async Task<IActionResult> UpdateBook(UpdateBookCommand book, CancellationToken cancellationToken)
        {
            await mediator.Send(book , cancellationToken);
            return Ok("Book Updated Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteBook/{id}")]

        public async Task<IActionResult> DeleteBook(int id , CancellationToken cancellationToken)
        {
            string Message = await mediator.Send(new DeleteCommand() {BookId = id } , cancellationToken);
            return Ok(Message);
        }
        [Authorize(Roles = "member")]
        [HttpGet("GetAvailableBook")]
        public async Task<IActionResult> GetAvailableBooks(CancellationToken cancellationToken)
        {
            List<GetAvailableBooksDto> AllAvailableBooks = await mediator.Send(new GetAvailableBooksQuery());
            return Ok(AllAvailableBooks);
        }

    }
}
