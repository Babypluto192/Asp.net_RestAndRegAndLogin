using DataAccess.Interfaces;
using DataAccess.Models;
using WebApplication2.interfaces;

namespace WebApplication2.Service;

public class BookService: IBookService
{
    private readonly IBookRepository _repository;


    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }


    public  async Task<List<BookEntity>> GetBooks()
    {
        return await _repository.GetAllBooksAsync();
    }

    public async Task<BookEntity?> GetBookById(Guid id)
    {   
        var book = await _repository.GetBookByIdAsync(id);
        return book;
        
    }

    public async Task<bool> AddBook(BodyRequestBook newbook)
    {
        var book = new BookEntity()
        {
            Id = Guid.NewGuid(),
            Title = newbook.Title,
            Author = newbook.Author,
            Description = newbook.Description,
        };
        return await _repository.CreateBookAsync(book);
    }

    public async Task<bool> UpdateBook(Guid id, BodyRequestBook book)
    {
        return await _repository.UpdateBookAsync(id, book);
    }

    public async Task<bool> DeleteBook(Guid id)
    {
        return await _repository.DeleteBookAsync(id);
    }
}