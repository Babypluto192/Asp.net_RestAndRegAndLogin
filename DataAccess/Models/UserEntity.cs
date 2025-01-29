using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace DataAccess.Models;

public class BaseUserEntity
{
    [Required (AllowEmptyStrings = false), Length(minimumLength:3, maximumLength:int.MaxValue)]
    public string Username { get; set; }
    
    
    [Required (AllowEmptyStrings = false), EmailAddress]
    public string Email { get; set; }
    
    [Required(AllowEmptyStrings = false), MinLength(4), MaxLength(int.MaxValue)]
    protected internal string Password { get; set; }


  
}

public class RegisterRequestUser: LoginRequestUser
{
    [Required (AllowEmptyStrings = false), Length(minimumLength:3, maximumLength:int.MaxValue)]
    public string Username { get; set; }
    
}


public class LoginRequestUser
{
    [Required (AllowEmptyStrings = false), EmailAddress]
    public string Email { get; set; }
    
    [Required(AllowEmptyStrings = false), MinLength(4), MaxLength(40)]
    public string Password { get; set; }
}

public class UserEntity : BaseUserEntity
{   
    public Guid Id { get; set; }
    
    public void SetPassword(string password)
    {
        Password =  BCrypt.Net.BCrypt.HashPassword(password);
    }

    public virtual bool ValidatePassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, this.Password);
    }
    
    
}
   
    


