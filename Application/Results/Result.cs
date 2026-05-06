using Domain.Enums;

namespace Application.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Data { get; set; }
    public string? Error { get; }
    public ErrorType? ErrorType { get; }

    private Result(T? data)
    {
        IsSuccess = true;
        Data = data;
    }

    private Result(string error, ErrorType errorType)
    {
        IsSuccess = false;
        Data = default;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Ok(T? data) => new(data);
    public static Result<T> Fail(string error, ErrorType errorType = Domain.Enums.ErrorType.Unknown) => new(error, errorType);

    // ───── Хелперы ─────
    public static Result<T> NotFound(string error = "Not found")
        => new(error, Domain.Enums.ErrorType.NotFound);

    public static Result<T> Conflict(string error = "Already exists")
        => new(error, Domain.Enums.ErrorType.Conflict);

    public static Result<T> Unauthorized(string error = "Unauthorized")
        => new(error, Domain.Enums.ErrorType.Unauthorized);

    public static Result<T> Forbidden(string error = "Forbidden")
        => new(error, Domain.Enums.ErrorType.Forbidden);

    public static Result<T> ValidationError(string error)
        => new(error, Domain.Enums.ErrorType.Validation);
}