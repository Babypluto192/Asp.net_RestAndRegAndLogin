using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.interfaces;

namespace WebApplication2.Service;

public class JwtService: IJwtService
{
    private string _secretKey = "SecretKeyThatIsMoreThan16Characters";
    private readonly SymmetricSecurityKey _signingKey;
    
    
    public JwtService()
    {
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    }


    public string GenerateJwtToken(string email)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email) };
        
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Issuer",
            audience: "Audiences",
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );
        
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(string email)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email) };
        
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Issuer",
            audience: "Audiences",
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials
        );
        
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<string?> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = "Issuer",
            ValidateAudience = true,
            ValidAudience = "Audiences",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,

        };
        try
        {
            var claimsPrincipal = await Task.Run(() =>
                tokenHandler.ValidateToken(token, validationParameters, out _)
            );

            
            var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            return emailClaim?.Value; 
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return null;
        }
    }
    
}