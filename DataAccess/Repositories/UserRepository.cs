using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository: IUserRepository
{
    private readonly DbContext _dbContext;


    public UserRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<bool> CreateUser(UserEntity user)
    {
        try
        {
            await _dbContext.Users.AddAsync(user).ConfigureAwait(true);
            await _dbContext.SaveChangesAsync().ConfigureAwait(true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<UserEntity?> LoginUser(String email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
        if (user != null)
        {
            return user;
        }
        throw new UnauthorizedAccessException();
    }


    public async Task<bool> VerifyUniqueOfEmail(String email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

        if (user == null)
        {
            return true;
        }
        
        return false;
    }
}