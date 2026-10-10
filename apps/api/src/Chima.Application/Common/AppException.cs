namespace Chima.Application.Common;

public class AppException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public AppException(string code, string message, int statusCode = 400) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found")
        : base("not_found", message, 404) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message)
        : base("validation_error", message, 400) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base("unauthorized", message, 401) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base("conflict", message, 409) { }
}

public class RateLimitException : AppException
{
    public RateLimitException(string message = "Too many requests")
        : base("rate_limited", message, 429) { }
}

public class LockedException : AppException
{
    public LockedException(string message = "Account temporarily locked")
        : base("locked", message, 423) { }
}