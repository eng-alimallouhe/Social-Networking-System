using SNS.API.Attributes;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;

namespace SNS.API.Middleware;

public sealed class SessionAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionAuthenticationMiddleware> _logger;

    public SessionAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<SessionAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentUserService currentUserService,
        ISessionService sessionService)
    {
        var endpoint = context.GetEndpoint();

        var requiresSession =
            endpoint?.Metadata.GetMetadata<RequireSessionAttribute>() is not null;

        if (!requiresSession)
        {
            await _next(context);
            return;
        }

        var sessionId = currentUserService.SessionId;
        var userId = currentUserService.UserId;

        if (sessionId is null || userId is null)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            return;
        }

        var isValid = await sessionService.ValidateAndUpdateSessionAsync(
            sessionId.Value,
            userId.Value,
            context.RequestAborted);

        if (!isValid)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            return;
        }

        await _next(context);
    }
}