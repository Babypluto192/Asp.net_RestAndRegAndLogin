using WebApplication2.interfaces;

namespace WebApplication2.Middleware;

public class AuthGuardMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthGuardMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
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
        var authorizationToken = auth.ToString();
      
        if (string.IsNullOrWhiteSpace(authorizationToken) || !authorizationToken.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authorization header is missing or invalid.");
            return;
        }
        
      
        
        
        var token = authorizationToken.Substring("Bearer ".Length).Trim();
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var jwtService =  scope.ServiceProvider.GetRequiredService<IJwtService>();

            if (jwtService == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("JWT Service not available.");
                return;
            }

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