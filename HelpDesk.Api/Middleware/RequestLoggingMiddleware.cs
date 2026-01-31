using System.Security.Claims;

namespace HelpDeskAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timestamp = DateTime.UtcNow;
        var method = context.Request.Method;
        var endpoint = context.Request.Path;
        var user = context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "Anonymous";
        var role = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "N/A";

        _logger.LogInformation(
            "[{Timestamp}] {Method} {Endpoint} | User: {User} | Role: {Role}",
            timestamp, method, endpoint, user, role
        );

        await _next(context);
    }
}