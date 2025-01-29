using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IUserRepository
{
    public Task<bool> CreateUser(UserEntity user);
    
    public Task<UserEntity?> LoginUser(String email);

    public Task<bool> VerifyUniqueOfEmail(String email);
}