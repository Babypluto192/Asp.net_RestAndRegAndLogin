namespace WebApplication2.interfaces;

public interface IJwtService
{
    public string GenerateJwtToken(string email);

    public string GenerateRefreshToken(string email);


    public  Task<string?> ValidateToken(string token);


}