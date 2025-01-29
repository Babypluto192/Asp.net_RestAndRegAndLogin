using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.interfaces;
namespace WebApplication2.controller;

[ApiController]
[Route("api/book")]



public class Bookcontroller: ControllerBase, IBookContoller
{
   private readonly IBookService _bookService;

   public Bookcontroller(IBookService bookService)
   {
       _bookService = bookService;
   }

   
   protected virtual  IActionResult? CheckAuthorization()
   {
       var userEmail = HttpContext.Items["UserEmail"]?.ToString();

       if (string.IsNullOrEmpty(userEmail))
       {
           return Unauthorized();
       }

       return null;
   }
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBooks()
    {
        CheckAuthorization();
        try
        {
            var books = await _bookService.GetBooks();
            return Ok(books);
        }
        catch (Exception ex)
        {   
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }
    
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBook(Guid id)
    {   
        CheckAuthorization();
        if (id == Guid.Empty)
        {
            return BadRequest();
        }
        try
        {
            var book = await _bookService.GetBookById(id);
            if (book == null)
            {
                throw new BadHttpRequestException("Book not found");
            }
            return Ok(book);
            
        }
        catch (Exception ex)
        {   
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBook([FromBody] BodyRequestBook  bodyRequestBook)
    {   
        CheckAuthorization();
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            var result = await _bookService.AddBook(bodyRequestBook);
            return  result ? Created() : BadRequest();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBook(Guid id,[FromBody] BodyRequestBook  bodyRequestBook )
    {   
        CheckAuthorization();
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            var result = await _bookService.UpdateBook(id, bodyRequestBook);
            return  result ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        CheckAuthorization();
        try
        {
            var result = await _bookService.DeleteBook(id);
            return result ? NoContent() : BadRequest();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }
}