using DataAccess.Interfaces;
using DataAccess.Models;
using DataAccess.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.controller;
using WebApplication2.interfaces;
using WebApplication2.Service;

namespace WebApi.Test;

public class TestableBookController : Bookcontroller
{
    public TestableBookController(IBookService bookService) : base(bookService) {}

    protected override IActionResult? CheckAuthorization()
    {
        return null;
    }
}
public class FixtureTest : IDisposable
{
    public IBookRepository BookRepository { get; }
    public IBookService BookService { get;  }
    public IBookContoller BookContoller { get;  }


    public FixtureTest()
    {
        BookRepository = A.Fake<IBookRepository>();
        BookService = new BookService(BookRepository);
        BookContoller = new TestableBookController(BookService);
    }
    
    public void Dispose()
    {
       Console.WriteLine("Удаленно");
    }
}

public class BookControllerTest: IClassFixture<FixtureTest>
{
    private readonly FixtureTest _fixture;

    public BookControllerTest(FixtureTest fixture)
    {
        _fixture = fixture; 
    }

    
    
    [Fact]
    public async Task GetBooksTest_ShouldReturnAllBooksAndOk_WhenSuccess()
    {
        // Arrange
        var fakeBooks = A.CollectionOfDummy<BookEntity>(5);
        A.CallTo(() => _fixture.BookRepository.GetAllBooksAsync()).Returns(Task.FromResult((List<BookEntity>)fakeBooks));
        
        // Act
        var actionResult = await _fixture.BookContoller.GetBooks();

        // Assert
        Assert.IsType<OkObjectResult>(actionResult);
        var result =  actionResult as OkObjectResult;
        var resultValue = result.Value as List<BookEntity>;
        Assert.Equal(5, resultValue.Count);
        
        Assert.Equal(fakeBooks, resultValue);

    }

    [Fact]
    public async Task GetBooks_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        A.CallTo(() => _fixture.BookRepository.GetAllBooksAsync())
            .ThrowsAsync(new Exception("Database error"));
        
        // Act
        var actionResult = await _fixture.BookContoller.GetBooks();

        // Assert
        Assert.IsType<BadRequestResult>(actionResult);
    }

    [Fact]
    public async Task GetBookByIdTest_ShouldReturnBookAndOk_WhenSuccess()
    {
        // Arrange

        var fakeBook = new BookEntity()
        {
            Id = Guid.NewGuid(), Description = "dd", Title = "dd", Author = "ddd"
        };
        
        A.CallTo(() => _fixture.BookRepository.GetBookByIdAsync(fakeBook.Id)).Returns(fakeBook);
        
        //Act
        
        var actionResult = await _fixture.BookContoller.GetBook(fakeBook.Id);
        
        // Assert
        
        Assert.IsType<OkObjectResult>(actionResult);
        var result = actionResult as OkObjectResult;
        var resultValue = result.Value as BookEntity;
        Assert.Equal(fakeBook.Id, resultValue.Id);
        Assert.Equal(fakeBook.Title, resultValue.Title);
        
    }

     [Fact]
   public async Task GetBookById_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var fakeBook = new BookEntity()
        {
            Id = Guid.NewGuid(), Description = "dd", Title = "dd", Author = "ddd"
        };
      A.CallTo(() => _fixture.BookRepository.GetBookByIdAsync(fakeBook.Id)).ThrowsAsync(new Exception("Database error"));
      
      // Act
      
      var actionResult = await _fixture.BookContoller.GetBook(fakeBook.Id);
      
      // Assert
      Assert.IsType<BadRequestResult>(actionResult);
   }


   [Fact]
   public async Task GetBookById_ShouldReturnBadRequest_WhenIdIsInvalid()
   {    
       // Arrange
       
       var fakeId = Guid.Empty;
        
      
       
       // Act
       
       var actionResult = await _fixture.BookContoller.GetBook(fakeId);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }

   [Fact]
   public async Task GetBookById_ShouldReturnBadRequest_WhenThereIsNoBook()
   {    
       // Arrange
       
       var fakeId = Guid.Empty;
       
       A.CallTo(() => _fixture.BookRepository.GetBookByIdAsync(fakeId)).Returns(Task.FromResult((BookEntity?)null));
       
       // Act
       
       var actionResult = await _fixture.BookContoller.GetBook(fakeId);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }


   [Fact]
   public async Task AddBookTest_ShouldReturnCreated_WhenBookIsAddedSuccessfully()
   {    
       // Arrange
       
       var fakeBook = A.Dummy<BodyRequestBook>();
       var fakeService = A.Fake<IBookService>();
       A.CallTo(() => fakeService.AddBook(fakeBook)).Returns(true);
       var fakeBookController = new TestableBookController(fakeService);
    
       // Act 
       
        var actionResult = await fakeBookController.AddBook(fakeBook);
       
       // Assert
       
        Assert.IsType<CreatedResult>(actionResult);
       
   }

   [Fact]
   public async Task AddBookTest_ShouldReturnBadRequest_WhenExceptionIsThrown() 
   {
       // Arrange
       
       var fakeBook = A.Dummy<BookEntity>();
       A.CallTo(() => _fixture.BookRepository.CreateBookAsync(fakeBook)).ThrowsAsync(new Exception("Database error"));
       
       // Act
       
       var actionResult = await _fixture.BookContoller.GetBook(fakeBook.Id);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }

   [Fact]
   public async Task AddBookTest_ShouldReturnBadRequest_WhenBookIsNotAddedSuccessfully()
   {    
       // Arrange
       
       var fakeBook = A.Dummy<BodyRequestBook>();
       var fakeService = A.Fake<IBookService>();
       var fakeBookController = new TestableBookController(fakeService);
       A.CallTo(() => fakeService.AddBook(fakeBook)).Returns(false);
       
       // Act
       
       var actionResult = await fakeBookController.AddBook(fakeBook);
       
       // Assert 
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }

   [Fact]
   public async Task UpdateBookTest_ShouldReturnOK_WhenBookisUpdatedSuccessfully()
   {
       // Arrange
       
       var fakeBook = A.Dummy<BodyRequestBook>();
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.UpdateBookAsync(fakeId, fakeBook)).Returns(true);
       
       // Act
       
       var actionResult = await _fixture.BookContoller.UpdateBook(fakeId, fakeBook);
       
       // Assert
       
       Assert.IsType<OkResult>(actionResult);
       
   }
   
   
   [Fact]
   public async Task UpdateBookTest_ShouldReturnBadRequest_WhenExceptionIsThrown()
   {
       // Arrange
       
       var fakeBook = A.Dummy<BodyRequestBook>();
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.UpdateBookAsync(fakeId, fakeBook)).ThrowsAsync(new Exception("Database error"));
       
       // Act
       
       var actionResult = await _fixture.BookContoller.UpdateBook(fakeId, fakeBook);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }
   
   
   [Fact]
   public async Task UpdateBookTest_ShouldReturnBadRequest_WhenBookIsNotUpdatedSuccessfully()
   {
       // Arrange
       
       var fakeBook = A.Dummy<BodyRequestBook>();
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.UpdateBookAsync(fakeId, fakeBook)).Returns(false);
       
       // Act
       
       var actionResult = await _fixture.BookContoller.UpdateBook(fakeId, fakeBook);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
       
   }

   [Fact]

   public async Task DeleteBookTest_ShouldReturnNoContent_WhenBookIsDeletedSuccessfully()
   {    
       // Arrange
       
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.DeleteBookAsync(fakeId)).Returns(true);
       
       // Act
       
       var actionResult = await _fixture.BookContoller.DeleteBook(fakeId);
       
       // Assert
       
       Assert.IsType<NoContentResult>(actionResult);
       
   }

   [Fact]
   public async Task DeleteBookTest_ShouldReturnBadRequest_WhenExceptionIsThrown()
   {
       // Arrange
       
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.DeleteBookAsync(fakeId)).ThrowsAsync(new Exception("Database error"));
       
       // Act
       
       var actionResult = await _fixture.BookContoller.DeleteBook(fakeId);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
   }

   [Fact]
   public async Task DeleteBookTest_ShouldReturnBadRequest_WhenBookIsNotDeletedSuccessfully()
   {
       // Arrange
       
       var fakeId = Guid.NewGuid();
       A.CallTo(() => _fixture.BookRepository.DeleteBookAsync(fakeId)).Returns(false);
       
       // Act
       
       var actionResult = await _fixture.BookContoller.DeleteBook(fakeId);
       
       // Assert
       
       Assert.IsType<BadRequestResult>(actionResult);
   } 
   
}