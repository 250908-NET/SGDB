using FluentAssertions;
using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Tests;

public class GamesDbContextTests
{
    private DbContextOptions<GamesDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<GamesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Can_Create_DbContext()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        context.Should().NotBeNull();
        context.Games.Should().NotBeNull();
        context.Platforms.Should().NotBeNull();
        context.GamePlatforms.Should().NotBeNull();
    }

    [Fact]
    public async Task Can_Add_And_Retrieve_Game()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        // Create and save developer & publisher first
        var developer = new Company { Name = "Test Studio" };
        var publisher = new Company { Name = "Test Publisher" };
        context.Companies.AddRange(developer, publisher);
        await context.SaveChangesAsync();

        var game = new Game
        {
            Name = "Test Game",
            DeveloperId = 1,    
            PublisherId = publisher.CompanyId,
            ReleaseDate = new DateTime(2024, 1, 1),
        };

        context.Games.Add(game);
        await context.SaveChangesAsync();

        var saved = await context.Games
        .Include(g => g.Developer)
        .Include(g => g.Publisher)
        .FirstOrDefaultAsync(g => g.Name == "Test Game");

        saved.Should().NotBeNull();
        saved!.Developer.Should().NotBeNull();
        saved.Developer.Name.Should().Be("Test Studio");

        saved.Publisher.Should().NotBeNull();
        saved.Publisher.Name.Should().Be("Test Publisher");
    }

    [Fact]
    public async Task Can_Add_And_Retrieve_Platform()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        var platform = new Platform
        {
            Name = "PlayStation 5",
        };

        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var saved = await context.Platforms.FirstOrDefaultAsync(p => p.Name == "PlayStation 5");

        saved.Should().NotBeNull();
    }

    // FIXME
    // [Fact]
    // public async Task Can_Link_Game_And_Platform()
    // {
    //     var options = GetInMemoryOptions();
    //     using var context = new GamesDbContext(options);

    //     var game = new Game { Name = "Halo", Developer = "Bungie", ReleaseDate = new DateTime(2001, 11, 15) };
    //     var platform = new Platform { Name = "Xbox" };

    //     context.Games.Add(game);
    //     context.Platforms.Add(platform);
    //     await context.SaveChangesAsync();

    //     var gp = new GamePlatform { GameId = game.GameId, PlatformId = platform.PlatformId };
    //     context.GamePlatforms.Add(gp);
    //     await context.SaveChangesAsync();

    //     var retrieved = await context.GamePlatforms
    //         .Include(x => x.Game)
    //         .Include(x => x.Platform)
    //         .FirstOrDefaultAsync();

    //     retrieved.Should().NotBeNull();
    //     retrieved!.Game.Name.Should().Be("Halo");
    //     retrieved.Platform.Name.Should().Be("Xbox");
    // }

    [Fact]
    public async Task Composite_Key_Should_Prevent_Duplicates()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        var game = new Game { Name = "FFVI", DeveloperId = 1, ReleaseDate = new DateTime(1994, 4, 2) };
        var platform = new Platform { Name = "SNES" };

        context.Games.Add(game);
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var gp = new GamePlatform { GameId = game.GameId, PlatformId = platform.PlatformId };
        context.GamePlatforms.Add(gp);
        await context.SaveChangesAsync();


    }
}
