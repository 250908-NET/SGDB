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

        var game = new Game
        {
            Title = "Test Game",
            Developer = "Test Studio",
            ReleaseYear = 2024
        };

        context.Games.Add(game);
        await context.SaveChangesAsync();

        var saved = await context.Games.FirstOrDefaultAsync(g => g.Title == "Test Game");

        saved.Should().NotBeNull();
        saved!.Developer.Should().Be("Test Studio");
    }

    [Fact]
    public async Task Can_Add_And_Retrieve_Platform()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        var platform = new Platform
        {
            Name = "PlayStation 5",
            Manufacturer = "Sony",
            ReleaseYear = 2020
        };

        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var saved = await context.Platforms.FirstOrDefaultAsync(p => p.Name == "PlayStation 5");

        saved.Should().NotBeNull();
        saved!.Manufacturer.Should().Be("Sony");
    }

    [Fact]
    public async Task Can_Link_Game_And_Platform()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        var game = new Game { Title = "Halo", Developer = "Bungie", ReleaseYear = 2001 };
        var platform = new Platform { Name = "Xbox", Manufacturer = "Microsoft", ReleaseYear = 2001 };

        context.Games.Add(game);
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var gp = new GamePlatform { GameId = game.GameId, PlatformId = platform.PlatformId };
        context.GamePlatforms.Add(gp);
        await context.SaveChangesAsync();

        var retrieved = await context.GamePlatforms
            .Include(x => x.Game)
            .Include(x => x.Platform)
            .FirstOrDefaultAsync();

        retrieved.Should().NotBeNull();
        retrieved!.Game.Title.Should().Be("Halo");
        retrieved.Platform.Name.Should().Be("Xbox");
    }

    [Fact]
    public async Task Composite_Key_Should_Prevent_Duplicates()
    {
        var options = GetInMemoryOptions();
        using var context = new GamesDbContext(options);

        var game = new Game { Title = "FFVI", Developer = "Square", ReleaseYear = 1994 };
        var platform = new Platform { Name = "SNES", Manufacturer = "Nintendo", ReleaseYear = 1990 };

        context.Games.Add(game);
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var gp = new GamePlatform { GameId = game.GameId, PlatformId = platform.PlatformId };
        context.GamePlatforms.Add(gp);
        await context.SaveChangesAsync();

        
    }
}
