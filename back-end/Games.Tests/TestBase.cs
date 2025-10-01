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
        };
        var ds = new Platform
        {
            PlatformId = 2,
            Name = "Nintendo DS",
        };

        // Seed companies
        var square = new Company
        {
            CompanyId = 1,
            Name = "Square",
        };
        var gameFreak = new Company
        {
            CompanyId = 2,
            Name = "Game Freak",
        };

        // Seed genres
        var rpg = new Genre
        {
            GenreId = 1,
            Name = "RPG",
        };

        // Seed games
        var finalFantasy6 = new Game
        {
            GameId = 1,
            Name = "Final Fantasy VI",
            Developer = square,
            ReleaseDate = new DateTime(1994, 4, 2),
        };
        var pokemonPlatinum = new Game
        {
            GameId = 2,
            Name = "Pokémon Platinum",
            Developer = gameFreak,
            ReleaseDate = new DateTime(2008, 9, 13),
        };

        // Seed game-platform join table
        var gp1 = new GamePlatform { GameId = 1, PlatformId = 1 };
        var gp2 = new GamePlatform { GameId = 2, PlatformId = 2 };

        // Seed game-genre join table
        var gg1 = new GameGenre { GameId = 1, GenreId = 1 };
        var gg2 = new GameGenre { GameId = 2, GenreId = 1 };

        Context.Platforms.AddRange(snes, ds);
        Context.Companies.AddRange(square, gameFreak);
        Context.Games.AddRange(finalFantasy6, pokemonPlatinum);
        Context.GamePlatforms.AddRange(gp1, gp2);

        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
