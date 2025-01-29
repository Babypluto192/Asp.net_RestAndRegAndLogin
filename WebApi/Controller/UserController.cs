using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.interfaces;

namespace WebApplication2.controller;

[ApiController]
[Route("/user/")]
public class UserController: ControllerBase, IUserController
{   
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public UserController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }
    
    private CookieOptions _cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(30)
    };
    
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestUser user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var isRegistered = await _userService.RegisterUser(user);
            var token = _jwtService.GenerateJwtToken(user.Email);
            var refreshToken =  _jwtService.GenerateRefreshToken(user.Email);
            if (isRegistered)
            {   
                HttpContext.Response.Headers.Append("Authorization", $"Bearer {token}");
                HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");
                HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, _cookieOptions);
                    
                return Created();
            }
            
            return BadRequest();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequestUser user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var isLoggedIn = await _userService.LoginUser(user);

            if (!isLoggedIn)
            {
                return BadRequest();
            }
            
            var token = _jwtService.GenerateJwtToken(user.Email);
            var refreshToken =  _jwtService.GenerateRefreshToken(user.Email);
            HttpContext.Response.Headers.Append("Authorization", $"Bearer {token}");
            HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");
            HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, _cookieOptions);
            return Ok();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }
    
    [AllowAnonymous]
    [HttpGet("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? refreshToken);
            if (refreshToken == null)
            {
                return BadRequest();
            }
            var isTokenValid =await _jwtService.ValidateToken(refreshToken);

            if (isTokenValid == null)
            {
                return BadRequest();
            }
            var newJwtToken = _jwtService.GenerateRefreshToken(refreshToken);
            var newRefreshToken =  _jwtService.GenerateRefreshToken(newJwtToken);
            HttpContext.Response.Headers.Append("Authorization", $"Bearer {newJwtToken}");
            HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, _cookieOptions);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}