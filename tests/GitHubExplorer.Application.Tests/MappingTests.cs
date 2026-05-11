using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Services;
using GitHubExplorer.Domain.Models;
using Shouldly;

namespace GitHubExplorer.Application.Tests;

public class MappingTests
{
    [Fact]
    public void UserProfile_ToDto_MapsAllProperties()
    {
        var user = new UserProfile
        {
            Login = "octocat",
            Name = "The Octocat",
            AvatarUrl = "https://avatars.githubusercontent.com/u/1",
            Bio = "GitHub mascot",
            Followers = 1000,
            PublicRepos = 50,
            HtmlUrl = "https://github.com/octocat",
        };

        // Use reflection to invoke the private MapToDto method
        var method = typeof(GitHubService).GetMethod("MapToDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.ShouldNotBeNull();
        var dto = (UserProfileDto)method!.Invoke(null, [user])!;

        dto.Login.ShouldBe("octocat");
        dto.Name.ShouldBe("The Octocat");
        dto.AvatarUrl.ShouldBe("https://avatars.githubusercontent.com/u/1");
        dto.Bio.ShouldBe("GitHub mascot");
        dto.Followers.ShouldBe(1000);
        dto.PublicRepos.ShouldBe(50);
        dto.HtmlUrl.ShouldBe("https://github.com/octocat");
    }

    [Fact]
    public void Repository_ToDto_MapsAllProperties()
    {
        var repo = new Repository
        {
            Name = "hello-world",
            Description = "My first repo",
            StargazersCount = 42,
            ForksCount = 7,
            Language = "TypeScript",
            HtmlUrl = "https://github.com/octocat/hello-world",
        };

        var method = typeof(GitHubService).GetMethod("MapToDtos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.ShouldNotBeNull();
        var dtos = (IReadOnlyList<RepositoryDto>)method!.Invoke(null, [new List<Repository> { repo }])!;

        dtos.Count.ShouldBe(1);
        dtos[0].Name.ShouldBe("hello-world");
        dtos[0].Description.ShouldBe("My first repo");
        dtos[0].StargazersCount.ShouldBe(42);
        dtos[0].ForksCount.ShouldBe(7);
        dtos[0].Language.ShouldBe("TypeScript");
        dtos[0].HtmlUrl.ShouldBe("https://github.com/octocat/hello-world");
    }

    [Fact]
    public void Repository_ToDto_PreservesOrderFromInput()
    {
        var repos = new List<Repository>
        {
            new() { Name = "first", Description = "A", StargazersCount = 10, ForksCount = 1, Language = "C#", HtmlUrl = "https://github.com/u/first" },
            new() { Name = "second", Description = "B", StargazersCount = 100, ForksCount = 10, Language = "TypeScript", HtmlUrl = "https://github.com/u/second" },
        };

        var method = typeof(GitHubService).GetMethod("MapToDtos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var dtos = (IReadOnlyList<RepositoryDto>)method!.Invoke(null, [repos])!;

        dtos.Count.ShouldBe(2);
        dtos[0].Name.ShouldBe("first");
        dtos[1].Name.ShouldBe("second");
        dtos[0].StargazersCount.ShouldBe(10);
        dtos[1].StargazersCount.ShouldBe(100);
    }

    [Fact]
    public void PaginatedResultDto_HoldsItemsAndTotalCount()
    {
        var items = new List<RepositoryDto>
        {
            new("repo1", "Desc", 10, 1, "C#", "https://github.com/u/r1"),
        };

        var paginated = new PaginatedResultDto<RepositoryDto>(items, 100);

        paginated.Items.Count.ShouldBe(1);
        paginated.Items[0].Name.ShouldBe("repo1");
        paginated.TotalCount.ShouldBe(100);
    }
}
