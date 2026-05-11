using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Domain.Tests;

public class ModelTests
{
    [Fact]
    public void UserProfile_CanConstructWithAllProperties()
    {
        var profile = new UserProfile
        {
            Login = "octocat",
            Name = "The Octocat",
            AvatarUrl = "https://avatars.githubusercontent.com/u/1",
            Bio = "GitHub mascot",
            Followers = 1000,
            PublicRepos = 50,
            HtmlUrl = "https://github.com/octocat",
        };

        profile.Login.ShouldBe("octocat");
        profile.Name.ShouldBe("The Octocat");
        profile.AvatarUrl.ShouldBe("https://avatars.githubusercontent.com/u/1");
        profile.Bio.ShouldBe("GitHub mascot");
        profile.Followers.ShouldBe(1000);
        profile.PublicRepos.ShouldBe(50);
        profile.HtmlUrl.ShouldBe("https://github.com/octocat");
    }

    [Fact]
    public void UserProfile_NullableProperties_AcceptNull()
    {
        var profile = new UserProfile
        {
            Login = "octocat",
            Name = null,
            AvatarUrl = "https://avatars.githubusercontent.com/u/1",
            Bio = null,
            Followers = 0,
            PublicRepos = 0,
            HtmlUrl = "https://github.com/octocat",
        };

        profile.Name.ShouldBeNull();
        profile.Bio.ShouldBeNull();
    }

    [Fact]
    public void Repository_CanConstructWithAllProperties()
    {
        var repo = new Repository
        {
            Name = "hello-world",
            Description = "A test repo",
            StargazersCount = 42,
            ForksCount = 7,
            Language = "TypeScript",
            HtmlUrl = "https://github.com/octocat/hello-world",
        };

        repo.Name.ShouldBe("hello-world");
        repo.Description.ShouldBe("A test repo");
        repo.StargazersCount.ShouldBe(42);
        repo.ForksCount.ShouldBe(7);
        repo.Language.ShouldBe("TypeScript");
        repo.HtmlUrl.ShouldBe("https://github.com/octocat/hello-world");
    }

    [Fact]
    public void Repository_NullableProperties_AcceptNull()
    {
        var repo = new Repository
        {
            Name = "hello-world",
            Description = null,
            StargazersCount = 0,
            ForksCount = 0,
            Language = null,
            HtmlUrl = "https://github.com/octocat/hello-world",
        };

        repo.Description.ShouldBeNull();
        repo.Language.ShouldBeNull();
    }

    [Fact]
    public void GitHubError_EnumValues_AreDefined()
    {
        GitHubError.NotFound.ShouldBe(GitHubError.NotFound);
        GitHubError.RateLimited.ShouldBe(GitHubError.RateLimited);
        GitHubError.NetworkError.ShouldBe(GitHubError.NetworkError);
        GitHubError.EmptyResult.ShouldBe(GitHubError.EmptyResult);
        GitHubError.Unknown.ShouldBe(GitHubError.Unknown);
    }

    [Fact]
    public void GitHubError_Enum_HasExpectedCount()
    {
        var values = Enum.GetValues<GitHubError>();
        values.Length.ShouldBe(5);
    }
}
