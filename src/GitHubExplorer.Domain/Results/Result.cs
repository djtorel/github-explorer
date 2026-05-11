namespace GitHubExplorer.Domain.Results;

public readonly struct Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed Result.");

    public GitHubError Error => IsFailure
        ? _error
        : throw new InvalidOperationException("Cannot access Error on a successful Result.");

    private readonly T? _value;
    private readonly GitHubError _error;

    private Result(bool isSuccess, T? value, GitHubError error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    public static Result<T> Success(T value) => new(true, value, default);

    public static Result<T> Failure(GitHubError error) => new(false, default, error);

    public Result<TResult> Map<TResult>(Func<T, TResult> map) =>
        IsSuccess
            ? Result<TResult>.Success(map(_value!))
            : Result<TResult>.Failure(_error);

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind) =>
        IsSuccess
            ? bind(_value!)
            : Result<TResult>.Failure(_error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<GitHubError, TResult> onFailure) =>
        IsSuccess
            ? onSuccess(_value!)
            : onFailure(_error);

    public Result<T> MapError(GitHubError error) =>
        IsFailure
            ? Failure(error)
            : this;
}
