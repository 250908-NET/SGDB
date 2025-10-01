using Games.Data;
using Games.Models;

namespace Games.Tests;

public static class Utilities
{
    public static async Task SeedTestDbAsync(GamesDbContext dbContext)
    {
        dbContext.Platforms.RemoveRange(dbContext.Platforms);
        dbContext.Companies.RemoveRange(dbContext.Companies);
        dbContext.Genres.RemoveRange(dbContext.Genres);
        dbContext.Games.RemoveRange(dbContext.Games);
        dbContext.GamePlatforms.RemoveRange(dbContext.GamePlatforms);
        dbContext.GameGenres.RemoveRange(dbContext.GameGenres);
        await dbContext.SaveChangesAsync();

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

        dbContext.Platforms.AddRange(snes, ds);
        dbContext.Companies.AddRange(square, gameFreak);
        dbContext.Genres.AddRange(rpg);
        dbContext.Games.AddRange(finalFantasy6, pokemonPlatinum);
        dbContext.GamePlatforms.AddRange(gp1, gp2);
        dbContext.GameGenres.AddRange(gg1, gg2);
        await dbContext.SaveChangesAsync();
    }
}
