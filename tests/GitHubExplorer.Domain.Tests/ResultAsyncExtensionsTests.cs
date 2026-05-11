using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Domain.Tests;

public class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task MapAsync_OnSuccess_TransformsValue()
    {
        var task = Task.FromResult(Result<int>.Success(21));
        var result = await task.MapAsync(x => x * 2);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task MapAsync_OnFailure_PassesThroughError()
    {
        var task = Task.FromResult(Result<int>.Failure(GitHubError.NotFound));
        var result = await task.MapAsync(x => x * 2);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public async Task BindAsync_OnSuccess_FlattensToInnerResult()
    {
        var task = Task.FromResult(Result<int>.Success(21));
        var result = await task.BindAsync(x => Result<int>.Success(x * 2));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task BindAsync_OnFailure_PassesThroughError()
    {
        var task = Task.FromResult(Result<int>.Failure(GitHubError.NotFound));
        var result = await task.BindAsync(x => Result<int>.Success(x * 2));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public async Task MapAsync_WithAsyncInput_AwaitsAndTransforms()
    {
        var task = Task.Run(async () =>
        {
            await Task.Delay(1);
            return Result<int>.Success(21);
        });

        var result = await task.MapAsync(x => x * 2);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task BindAsync_WithAsyncInput_AwaitsAndBinds()
    {
        var task = Task.Run(async () =>
        {
            await Task.Delay(1);
            return Result<int>.Success(21);
        });

        var result = await task.BindAsync(x => Result<int>.Success(x * 2));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task Chain_MapAsync_Then_BindAsync_ComputesCorrectValue()
    {
        var task = Task.FromResult(Result<int>.Success(5));
        var result = await task
            .MapAsync(x => x * 2)
            .BindAsync(x => Result<int>.Success(x + 1));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(11);
    }
}
