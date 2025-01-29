using DataAccess.Interfaces;
using DataAccess.Models;
using DataAccess.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.controller;
using WebApplication2.interfaces;
using WebApplication2.Service;

namespace WebApi.Test;

public class FixturesForTest: IDisposable
{
    public IUserRepository UserRepository { get; } 
    
    public IUserService UserService { get; }
    
    public IUserController UserController { get; }
    
    public IJwtService JwtService { get; }

    public FixturesForTest()
    {
        UserRepository = A.Fake<IUserRepository>();
        UserService = new UserService(UserRepository);
        JwtService = A.Fake<IJwtService>();
        UserController = new UserController(UserService,JwtService ) 
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
    
    public void Dispose()
    {
        Console.WriteLine("Удаленно");
    }
}

public class UserControllerTest: IClassFixture<FixturesForTest>
{
    private readonly FixturesForTest _fixture;

    public UserControllerTest(FixturesForTest fixture)
    {
        _fixture = fixture;
    }

    
        [Fact]
        public async Task RegisterUser_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange 
            var fakeUserService = A.Fake<IUserService>();
            var fakeUserController = new UserController(fakeUserService, _fixture.JwtService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var fakeUser = A.Dummy<RegisterRequestUser>();
            A.CallTo(() => fakeUserService.RegisterUser(fakeUser)).Returns(true);
            A.CallTo(() => _fixture.JwtService.GenerateJwtToken(fakeUser.Email)).Returns("fakeJwtToken");
            A.CallTo(() => _fixture.JwtService.GenerateRefreshToken(fakeUser.Email)).Returns("fakeRefreshToken");

            // Act
            var actionResult = await fakeUserController.RegisterUser(fakeUser);

            // Assert
            Assert.IsType<CreatedResult>(actionResult);
            var httpContext = fakeUserController.HttpContext;
            Assert.True(httpContext.Response.Headers.ContainsKey("Authorization"));
            Assert.Equal("Bearer fakeJwtToken", httpContext.Response.Headers["Authorization"].ToString());
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenUserIsNotRegistered()
        {
            // Arrange 
            var fakeUser = A.Dummy<RegisterRequestUser>();
            var fakeEntityUser = A.Dummy<UserEntity>();
            A.CallTo(() => _fixture.UserRepository.CreateUser(fakeEntityUser)).Returns(false);
            
            
            // Act 
            var actionResult = await _fixture.UserController.RegisterUser(fakeUser);
            
            // Assert
            Assert.IsType<BadRequestResult>(actionResult);
            
            
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenUserIsAlreadyRegistered()
        {
            // Arrange
            var fakeEntityUser = A.Dummy<UserEntity>();
            var fakeUser = A.Dummy<RegisterRequestUser>();
            A.CallTo(() => _fixture.UserRepository.CreateUser(fakeEntityUser)).Returns(true);
            A.CallTo(() => _fixture.UserRepository.VerifyUniqueOfEmail(fakeUser.Email)).Returns(false);
            
            // Act
            var actionResult = await _fixture.UserController.RegisterUser(fakeUser);
            
            // Assert
            Assert.IsType<BadRequestResult>(actionResult);
            
        }
        
        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenUserDataIsWrong()
        {
            var registerUser = new RegisterRequestUser();
            var userController = new UserController(_fixture.UserService, _fixture.JwtService);
            userController.ControllerContext = new ControllerContext();
            userController.ModelState.AddModelError("Email", "Wrong Email");
            
           
            // Act
            var actionResult = await  userController.RegisterUser(registerUser);
            
            // Assert 
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnOk_WhenUserIsLoggedIn()
        {
            // Arrange
            var loginUser = A.Dummy<LoginRequestUser>();
            var fakeUser = A.Fake<UserEntity>();
            
            A.CallTo(() => _fixture.UserRepository.LoginUser(loginUser.Email)).Returns(fakeUser);
            A.CallTo(() => fakeUser.ValidatePassword(loginUser.Password)).Returns(true);
            A.CallTo(() => _fixture.JwtService.GenerateJwtToken(fakeUser.Email)).Returns("fakeJwtToken");
            A.CallTo(() => _fixture.JwtService.GenerateRefreshToken(fakeUser.Email)).Returns("fakeRefreshToken");

            // Act
            var actionResult = await _fixture.UserController.LoginUser(loginUser);
            
            // Assert
            Assert.IsType<OkResult>(actionResult);


        }

        [Fact]
        public async Task LoginUser_ShouldReturnBadRequest_WhenUserIsNotLoggedIn()
        {   
            // Arrange
            var loginUser = A.Dummy<LoginRequestUser>();
            UserEntity? userEntity = null;
            A.CallTo(() => _fixture.UserRepository.LoginUser(loginUser.Email)).Returns(userEntity);
            
            // Act
            var actionResult = await _fixture.UserController.LoginUser(loginUser);


            // Assert
            Assert.IsType<BadRequestResult>(actionResult);
        }


        [Fact]
        public async Task LoginUser_ShouldReturnBadRequest_WhenUserDataIsWrong()
        {
            // Arrange 
            var loginUser = new LoginRequestUser();
            var userController = new UserController(_fixture.UserService, _fixture.JwtService);
            userController.ControllerContext = new ControllerContext();
            userController.ModelState.AddModelError("Email", "Wrong Email");
            
           
            // Act
            var actionResult = await  userController.LoginUser(loginUser);
            
            // Assert 
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }
        
        
}
