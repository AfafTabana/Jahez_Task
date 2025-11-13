using Jahez_Task.DTOs.BookForAdmin;
using Jahez_Task.DTOs.BookForMember;
using Jahez_Task.Services.BookService;
using Jahez_Task.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jahez_Task.Controllers
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
        public async Task<IActionResult> GetAllForMembers() {

            IEnumerable<DisplayBook> Books = await bookService.GetAll();
            return Ok(Books);        
        }
        [Authorize(Roles = "admin")]
        [HttpGet("DisplayBooksForAdmin")]

        public async Task<IActionResult> GetAllForAdmin()
        {
            IEnumerable<DIsplayBook> Books = await bookService.GetAllBookForAdmin();
            return Ok(Books);


        }
        [Authorize(Roles = "admin")]
        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetById(int Id )
        {
            var Book = await bookService.GetByIdForAdmin(Id);
            return Ok(Book);

        }
        [Authorize(Roles = "admin")]
        [HttpPost("AddBook")]

        public IActionResult AddBook(DIsplayBook book)
        {
            bookService.AddBook(book);
            return Ok("Book Added Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpPut("UpdateBook")]

        public IActionResult UpdateBook(DIsplayBook book)
        {
            bookService.UpdateBook(book);
            return Ok("Book Updated Succesfully");
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteBook")]

        public async Task<IActionResult> DeleteBook(int Id)
        {
            await bookService.DeleteBook(Id);
            return Ok("Book Deleted Successfully");
        }
        [Authorize(Roles = "member")]
        [HttpGet("GetAvailableBook")]
        public async Task<IActionResult> GetAvailableBooks()
        {
           List<DisplayBook> AllAvailableBooks = await bookService.GetAvailableBooks();
            return Ok(AllAvailableBooks);
        }
        [Authorize(Roles = "member")]
        [HttpPost("BorrowBook")]

        public IActionResult BorrowBook(int UserId, [FromBody]DisplayBook book) {

            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
                bookService.BorrowBook(UserId, book);
                return Ok("Book has been borrowed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "member")]
        [HttpPost("ReturnBook")]

        public async Task<IActionResult> ReturnBook(int UserId , DisplayBook book)
        {
            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
               await bookService.ReturnBook(UserId, book);
                return Ok("Book has been Returned successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
