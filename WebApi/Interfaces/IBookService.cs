using DataAccess.Models;

namespace WebApplication2.interfaces;

public interface IBookService
{
    public Task<List<BookEntity>> GetBooks();
    
    public Task<BookEntity?> GetBookById(Guid id);
    
    public Task<Boolean> AddBook(BodyRequestBook newbook);
    
    public Task<Boolean> UpdateBook(Guid id, BodyRequestBook book);
    
    public Task<Boolean> DeleteBook(Guid id);
    
}