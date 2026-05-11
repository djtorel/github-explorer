namespace GitHubExplorer.Domain.Results;

public static class ResultAsyncExtensions
{
    public static async Task<Result<TResult>> MapAsync<T, TResult>(
        this Task<Result<T>> task,
        Func<T, TResult> map)
    {
        var result = await task;
        return result.Map(map);
    }

    public static async Task<Result<TResult>> BindAsync<T, TResult>(
        this Task<Result<T>> task,
        Func<T, Result<TResult>> bind)
    {
        var result = await task;
        return result.Bind(bind);
    }
}
