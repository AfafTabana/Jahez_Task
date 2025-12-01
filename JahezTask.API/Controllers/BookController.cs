
using Jahez_Task.Services.BookService;
using JahezTask.Application.DTOs.Book;
using JahezTask.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JahezTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public readonly IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }
        [Authorize(Roles = "member")]
        [HttpGet("DisplayBooksForMembers")]
        public async Task<IActionResult> GetAllBooksForMembers(CancellationToken cancellationToken)
        {

            IEnumerable<DisplayBookForMember> Books = await bookService.GetAll(cancellationToken);
            return Ok(Books);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("DisplayBooksForAdmin")]

        public async Task<IActionResult> GetAllBooksForAdmin(CancellationToken cancellationToken)
        {
            IEnumerable<DisplayBookForAdmin> Books = await bookService.GetAllBookForAdmin(cancellationToken);
            return Ok(Books);


        }
        [Authorize(Roles = "admin")]
        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetBookById(int Id , CancellationToken cancellationToken)
        {
            var Book = await bookService.GetByIdForAdmin(Id , cancellationToken);
            return Ok(Book);

        }
        [Authorize(Roles = "admin")]
        [HttpPost("AddBook")]

        public async Task<IActionResult> AddBook(DisplayBookForAdmin book , CancellationToken cancellationToken)
        {
            await bookService.AddBook(book , cancellationToken);
            return Ok("Book Added Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpPut("UpdateBook")]

        public async Task<IActionResult> UpdateBook(DisplayBookForAdmin book, int BookId , CancellationToken cancellationToken)
        {
            await bookService.UpdateBook(book, BookId , cancellationToken);
            return Ok("Book Updated Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteBook/{id}")]

        public async Task<IActionResult> DeleteBook(int id , CancellationToken cancellationToken)
        {
            string Message = await bookService.DeleteBook(id , cancellationToken);
            return Ok(Message);
        }
        [Authorize(Roles = "member")]
        [HttpGet("GetAvailableBook")]
        public async Task<IActionResult> GetAvailableBooks(CancellationToken cancellationToken)
        {
            List<DisplayBookForMember> AllAvailableBooks = await bookService.GetAvailableBooks(cancellationToken);
            return Ok(AllAvailableBooks);
        }

    }
}
