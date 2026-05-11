using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Domain.Tests;

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
    public void Map_OnSuccess_TransformsValue()
    {
        var result = Result<int>.Success(21)
            .Map(x => x * 2);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Map_OnFailure_DoesNotCallMapper()
    {
        var result = Result<int>.Failure(GitHubError.NotFound)
            .Map(x => x * 2);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public void Bind_OnSuccess_FlattensToInnerResult()
    {
        var result = Result<int>.Success(21)
            .Bind(x => Result<int>.Success(x * 2));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Bind_OnFailure_DoesNotCallBinder()
    {
        var result = Result<int>.Failure(GitHubError.NotFound)
            .Bind(x => Result<int>.Success(x * 2));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public void Match_OnSuccess_ReturnsSuccessBranch()
    {
        var result = Result<int>.Success(21)
            .Match(
                onSuccess: x => x * 2,
                onFailure: _ => -1);

        result.ShouldBe(42);
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
    public void MapError_OnSuccess_ReturnsUnchanged()
    {
        var original = Result<int>.Success(42);
        var result = original.MapError(GitHubError.NetworkError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Success_WithNullReferenceType_ValueIsNull()
    {
        var result = Result<string>.Success(null!);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Success_WithValueType_ValueIsNotNull()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);

        var boolResult = Result<bool>.Success(true);
        boolResult.Value.ShouldBeTrue();
    }
}
