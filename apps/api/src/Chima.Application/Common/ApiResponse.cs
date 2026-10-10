namespace Chima.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiError> Errors { get; init; } = Array.Empty<ApiError>();

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Fail(string message, params ApiError[] errors) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}

public class ApiResponse : ApiResponse<object> { }

public record ApiError(string Code, string Message, string? Field = null);
