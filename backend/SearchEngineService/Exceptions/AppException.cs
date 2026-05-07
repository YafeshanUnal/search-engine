namespace SearchEngineService.Exceptions;

public class AppException(
    string errorCode,
    string message,
    int statusCode = StatusCodes.Status400BadRequest) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
}
