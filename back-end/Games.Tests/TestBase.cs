using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Tests;

public class TestBase : IDisposable
{
    protected GamesDbContext Context { get; private set; }

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<GamesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        Context = new GamesDbContext(options);
        Context.Database.EnsureCreated();
    }

    protected async Task SeedDataAsync()
    {
        // Seed platforms
        var snes = new Platform
        {
            PlatformId = 1,
            Name = "Super Nintendo",
            Manufacturer = "Nintendo",
            ReleaseYear = 1994
        };

        var ds = new Platform
        {
            PlatformId = 2,
            Name = "Nintendo DS",
            Manufacturer = "Nintendo",
            ReleaseYear = 2008
        };

        // Seed games
        var finalFantasy6 = new Game
        {
            GameId = 1,
            Title = "Final Fantasy VI",
            Developer = "Square",
            ReleaseYear = 1994
        };

        var pokemonPlatinum = new Game
        {
            GameId = 2,
            Title = "Pokémon Platinum",
            Developer = "Game Freak",
            ReleaseYear = 2008
        };

        // Seed join table
        var gp1 = new GamePlatform { GameId = 1, PlatformId = 1 };
        var gp2 = new GamePlatform { GameId = 2, PlatformId = 2 };

        Context.Platforms.AddRange(snes, ds);
        Context.Games.AddRange(finalFantasy6, pokemonPlatinum);
        Context.GamePlatforms.AddRange(gp1, gp2);

        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
