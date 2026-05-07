using System.Text.Json;
using SearchEngineService.Constants;
using SearchEngineService.Exceptions;
using SearchEngineService.Models;

namespace SearchEngineService.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            AppException appEx => (appEx.StatusCode, appEx.ErrorCode, appEx.Message),
            OperationCanceledException => (499, ErrorCodes.UnknownError, ErrorMessages.RequestCancelled),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError, ErrorMessages.UnexpectedError)
        };

        if (statusCode >= 500)
        {
            logger.LogError(exception, ErrorMessages.UnhandledException, context.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception. TraceId: {TraceId}", context.TraceIdentifier);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            StatusCode = statusCode,
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
