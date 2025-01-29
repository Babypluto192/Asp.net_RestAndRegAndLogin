using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BookRepository: IBookRepository
{
    private readonly DbContext _dbContext;
    
    
    public BookRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BookEntity>> GetAllBooksAsync()
    {
        return await _dbContext.Books.AsNoTracking().ToListAsync();
    }

    public async Task<BookEntity?> GetBookByIdAsync(Guid id)
    {
        return await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Boolean> CreateBookAsync(BookEntity book)
    {
        try
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
    }

    public async Task<Boolean> UpdateBookAsync(Guid id,BodyRequestBook bodyRequestBook)
    {
        try
        {
            await _dbContext.Books
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(b => 
                    b.SetProperty(bo => bo.Title, bodyRequestBook.Title)
                        .SetProperty(bo => bo.Author, bodyRequestBook.Author)
                        .SetProperty(bo => bo.Description, bodyRequestBook.Description)
                    );
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Boolean> DeleteBookAsync(Guid id)
    {
        try
        {
            await _dbContext.Books.Where(b => b.Id == id).ExecuteDeleteAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
      
    }
}