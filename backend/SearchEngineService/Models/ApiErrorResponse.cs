namespace SearchEngineService.Models;

public class ApiErrorResponse
{
    public required string ErrorCode { get; init; }
    public required string Message { get; init; }
    public required int StatusCode { get; init; }
    public required string TraceId { get; init; }
}
