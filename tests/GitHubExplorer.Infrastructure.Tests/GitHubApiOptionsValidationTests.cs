using Microsoft.Extensions.Options;
using Shouldly;

namespace GitHubExplorer.Infrastructure.Tests;

public class GitHubApiOptionsValidationTests
{
    private readonly GitHubApiOptionsValidation _validator = new();

    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess()
    {
        var options = new GitHubApiOptions
        {
            BaseUrl = "https://api.github.com",
            Token = null
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Validate_EmptyBaseUrl_ReturnsFailure()
    {
        var options = new GitHubApiOptions { BaseUrl = "" };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeFalse();
        (result.Failures ?? []).ShouldContain(f => f.Contains("BaseUrl is required"));
    }

    [Fact]
    public void Validate_NonHttpsBaseUrl_ReturnsFailure()
    {
        var options = new GitHubApiOptions { BaseUrl = "http://api.github.com" };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeFalse();
        (result.Failures ?? []).ShouldContain(f => f.Contains("HTTPS"));
    }

    [Fact]
    public void Validate_InvalidBaseUrl_ReturnsFailure()
    {
        var options = new GitHubApiOptions { BaseUrl = "not-a-url" };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeFalse();
        (result.Failures ?? []).ShouldContain(f => f.Contains("HTTPS"));
    }

    [Fact]
    public void Validate_ValidToken_ReturnsSuccess()
    {
        var options = new GitHubApiOptions
        {
            BaseUrl = "https://api.github.com",
            Token = "ghp_abcdefghijklmnopqrstuvwxyz"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ValidPatToken_ReturnsSuccess()
    {
        var options = new GitHubApiOptions
        {
            BaseUrl = "https://api.github.com",
            Token = "github_pat_xxxxxxxxxxxxxxxxxxxx"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Validate_InvalidToken_ReturnsFailure()
    {
        var options = new GitHubApiOptions
        {
            BaseUrl = "https://api.github.com",
            Token = "invalid_token"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeFalse();
        (result.Failures ?? []).ShouldContain(f => f.Contains("ghp_") && f.Contains("github_pat_"));
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        var options = new GitHubApiOptions
        {
            BaseUrl = "ftp://api.github.com",
            Token = "bad"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.ShouldBeFalse();
        (result.Failures ?? []).Count().ShouldBe(2);
    }
}
