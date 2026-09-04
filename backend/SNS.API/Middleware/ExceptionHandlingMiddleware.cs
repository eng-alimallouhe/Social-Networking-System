using SNS.Application.Abstractions.Loggings;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using System.Net;
using System.Text.Json;

namespace SNS.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAppLogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
            exception,
            context.TraceIdentifier,
            context.Request.Path,
            context.Request.Method);

            if (context.Response.HasStarted)
            {
                logger.LogWarning(
                    "Cannot handle exception because the response has already started. TraceId: {TraceId}",
                    context.TraceIdentifier);
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = Result.Failure(OperationStatusCode.ServerError);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}