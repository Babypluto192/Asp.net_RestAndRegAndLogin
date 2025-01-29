using WebApplication2.interfaces;

namespace WebApplication2.Middleware;

public class AuthGuardMiddleware
{
    private readonly RequestDelegate _next;
 

    public AuthGuardMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var path = context.Request.Path.Value;
        if (path.StartsWith("/user/login") || path.StartsWith("/user/register") || path.StartsWith("/swagger/") || path.StartsWith("/user/refresh"))
        {
            await _next(context);
            return;
        }
        context.Request.Headers.TryGetValue("Authorization", out var auth);
        Console.WriteLine(auth.ToString());
        var authorizationToken = context.Request.Headers["Authorization"].ToString();
      
        if (string.IsNullOrWhiteSpace(authorizationToken) || !authorizationToken.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authorization header is missing or invalid.");
            return;
        }
        
      
        
        
        var token = authorizationToken.Substring("Bearer ".Length).Trim();
        using (var scope = serviceProvider.CreateScope())
        {
            var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
            var email = await jwtService.ValidateToken(token);

            if (email == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
            context.Items["UserEmail"] = email;

        }

        
        
        await _next.Invoke(context);
    }
}