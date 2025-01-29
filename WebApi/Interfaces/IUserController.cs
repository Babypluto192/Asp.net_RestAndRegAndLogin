using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.interfaces;

public interface IUserController
{
    public Task<IActionResult> RegisterUser([FromBody] RegisterRequestUser user);
    
    public Task<IActionResult> LoginUser([FromBody] LoginRequestUser user);
    
    public Task<IActionResult> RefreshToken();
}