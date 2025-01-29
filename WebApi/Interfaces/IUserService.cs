using DataAccess.Models;

namespace WebApplication2.interfaces;

public interface IUserService
{
    public Task<bool> RegisterUser(RegisterRequestUser user);
    
    public Task<bool> LoginUser(LoginRequestUser user);
}