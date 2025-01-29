using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication2.interfaces;

public interface IBookContoller
{
    public Task<IActionResult>  GetBooks(); 
    
    public Task<IActionResult> GetBook(Guid id);
    
    public Task<IActionResult> AddBook(BodyRequestBook book);
    
    public Task<IActionResult> UpdateBook(Guid id, BodyRequestBook book);
   
    public Task<IActionResult> DeleteBook(Guid id);

    protected virtual IActionResult? CheckAuthorization()
    {
        return null;
    }

}