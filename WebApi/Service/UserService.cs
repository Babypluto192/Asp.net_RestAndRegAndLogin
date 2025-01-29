using DataAccess.Interfaces;
using DataAccess.Models;
using WebApplication2.interfaces;

namespace WebApplication2.Service;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    
    public async Task<bool> RegisterUser(RegisterRequestUser user)
    {
       var isEmailUnique = await _userRepository.VerifyUniqueOfEmail(user.Email);
       if (!isEmailUnique)
       {
           throw new Exception("Email already registered");
       }
       
       UserEntity userEntity = new UserEntity()
       {
           Id = Guid.NewGuid(),
           Email = user.Email,
           Username = user.Username,
       };
       userEntity.SetPassword(user.Password);
       
       return await _userRepository.CreateUser(userEntity);
        
    }

    public async Task<bool> LoginUser(LoginRequestUser user)
    {
        UserEntity? isUserExisting = await _userRepository.LoginUser(user.Email);

        if (isUserExisting == null)
        {
            return false;
        }
        
        return isUserExisting.ValidatePassword(user.Password);
        
    }
}