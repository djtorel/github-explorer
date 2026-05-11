using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Infrastructure.Tests;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessResult()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_CreatesFailureResult()
    {
        var result = Result<int>.Failure(GitHubError.NotFound);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        var result = Result<int>.Failure(GitHubError.NotFound);

        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Error_OnSuccess_ThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);

        Should.Throw<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Map_OnSuccess_TransformsValue()
    {
        var result = Result<int>.Success(42)
            .Map(x => x.ToString());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void Map_OnFailure_PassesThroughError()
    {
        var result = Result<int>.Failure(GitHubError.NotFound)
            .Map(x => x.ToString());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public void Bind_OnSuccess_ChainsToNextResult()
    {
        var result = Result<int>.Success(42)
            .Bind(x => Result<string>.Success(x.ToString()));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void Bind_OnFailure_PassesThroughError()
    {
        var result = Result<int>.Failure(GitHubError.NotFound)
            .Bind(x => Result<string>.Success(x.ToString()));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public void Bind_OnSuccess_WhenNextFails_ReturnsFailure()
    {
        var result = Result<int>.Success(42)
            .Bind(_ => Result<string>.Failure(GitHubError.RateLimited));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    [Fact]
    public void Match_OnSuccess_ReturnsSuccessBranch()
    {
        var result = Result<int>.Success(42)
            .Match(
                onSuccess: x => x * 2,
                onFailure: _ => -1);

        result.ShouldBe(84);
    }

    [Fact]
    public void Match_OnFailure_ReturnsFailureBranch()
    {
        var result = Result<int>.Failure(GitHubError.NotFound)
            .Match(
                onSuccess: x => x * 2,
                onFailure: _ => -1);

        result.ShouldBe(-1);
    }

    [Fact]
    public void MapError_OnFailure_ChangesError()
    {
        var result = Result<int>.Failure(GitHubError.Unknown)
            .MapError(GitHubError.NetworkError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NetworkError);
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal()
    {
        var original = Result<int>.Success(42);
        var result = original.MapError(GitHubError.NetworkError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }
}
