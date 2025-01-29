
using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IBookRepository
{
   public Task<List<BookEntity>> GetAllBooksAsync();
 
    public Task<BookEntity?> GetBookByIdAsync(Guid id);
    
   public Task<Boolean> CreateBookAsync(BookEntity book);
   
    public Task<Boolean> UpdateBookAsync(Guid id, BodyRequestBook bodyRequestBook);
    
     public Task<Boolean> DeleteBookAsync(Guid id);
}