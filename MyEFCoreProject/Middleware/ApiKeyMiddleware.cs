using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
    {
        if (context.Request.Path.Equals("/api/v1", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);  // Bypass middleware for api/v1
            return;
        }
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);  // Bypass middleware for Swagger
            return;
        }

        if (!context.Request.Headers.TryGetValue("API_KEY", out var extractedApiKey))
        {
            context.Response.StatusCode = 401; 
            await context.Response.WriteAsync("API Key is missing");
            await AuditLogService.LogAPIKeyAsync(context.Request.Method, "401 UNAUTHORIZED: Missing API Key", extractedApiKey);
            return;
        }

        if (!await Authorization.AuthorizeUser(extractedApiKey, context.Request.Method, dbContext))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access Denied: Invalid API Key");
            await AuditLogService.LogAPIKeyAsync(context.Request.Method, "403 FORBIDDEN: Invalid API Key", extractedApiKey);
            return;
        }

        await AuditLogService.LogAPIKeyAsync(context.Request.Method, "200 OK: Valid API Key used", extractedApiKey);
        await _next(context);
    }
}
