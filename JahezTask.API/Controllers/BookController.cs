
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
        //[Authorize(Roles = "member")]
        [HttpGet("DisplayBooksForMembers")]
        public async Task<IActionResult> GetAllForMembers() {

            IEnumerable<DisplayBookForMember> Books = await bookService.GetAll();
            return Ok(Books);        
        }
        //[Authorize(Roles = "admin")]
        [HttpGet("DisplayBooksForAdmin")]

        public async Task<IActionResult> GetAllForAdmin()
        {
            IEnumerable<DisplayBookForAdmin> Books = await bookService.GetAllBookForAdmin();
            return Ok(Books);


        }
        //[Authorize(Roles = "admin")]
        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetById(int Id )
        {
            var Book = await bookService.GetByIdForAdmin(Id);
            return Ok(Book);

        }
        //[Authorize(Roles = "admin")]
        [HttpPost("AddBook")]

        public IActionResult AddBook(DisplayBookForAdmin book)
        {
            bookService.AddBook(book);
            return Ok("Book Added Succesfully");
        }
        //[Authorize(Roles = "admin")]
        [HttpPut("UpdateBook")]

        public async Task<IActionResult> UpdateBook(DisplayBookForAdmin book , int BookId)
        {
            await bookService.UpdateBook(book , BookId);
            return Ok("Book Updated Succesfully");
        }
        //[Authorize(Roles = "admin")]
        [HttpDelete("DeleteBook/{Id}")]

        public async Task<IActionResult> DeleteBook(int Id)
        {
            string Message = await bookService.DeleteBook(Id);
            return Ok(Message);
        }
        //[Authorize(Roles = "member")]
        [HttpGet("GetAvailableBook")]
        public async Task<IActionResult> GetAvailableBooks()
        {
           List<DisplayBookForMember> AllAvailableBooks = await bookService.GetAvailableBooks();
            return Ok(AllAvailableBooks);
        }
        //[Authorize(Roles = "member")]
        [HttpPost("BorrowBook")]

        public async  Task<IActionResult> BorrowBook(int UserId, [FromBody]DisplayBookForMember book) {

            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
               var (Loan , message) = await bookService.BorrowBook(UserId, book);
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

        public async Task<IActionResult> ReturnBook(int UserId , DisplayBookForMember book)
        {
            if (book == null)
                return BadRequest("Book data is required.");

            try
            {
               var (loan , message )=await bookService.ReturnBook(UserId, book);
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
