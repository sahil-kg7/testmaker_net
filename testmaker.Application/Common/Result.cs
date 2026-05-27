namespace testmaker.Application.Common;

public enum ErrorType
{
    Unknown = 0,
    NotFound = 1,
    Conflict = 2,
    Validation = 3,
    Unauthorized = 4
}

public class Result
{
    protected Result(bool isSuccess, string? error, ErrorType errorType = ErrorType.Unknown)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(string error, ErrorType errorType = ErrorType.Unknown)
        => new(false, error, errorType);
}

public class Result<T> : Result
{
    private Result(T? value, bool isSuccess, string? error, ErrorType errorType = ErrorType.Unknown)
        : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value, true, null);
    public new static Result<T> Failure(string error, ErrorType errorType = ErrorType.Unknown)
        => new(default, false, error, errorType);
}
