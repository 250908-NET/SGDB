using Games.Data;
using Games.Models;

namespace Games.Tests;

public static class Utilities
{
    public static async Task SeedTestDbAsync(GamesDbContext dbContext)
    {
        dbContext.GamePlatforms.RemoveRange(dbContext.GamePlatforms);
        dbContext.GameGenres.RemoveRange(dbContext.GameGenres);
        dbContext.Games.RemoveRange(dbContext.Games);
        dbContext.Platforms.RemoveRange(dbContext.Platforms);
        dbContext.Companies.RemoveRange(dbContext.Companies);
        dbContext.Genres.RemoveRange(dbContext.Genres);
        await dbContext.SaveChangesAsync();

        // Seed platforms
        var snes = new Platform { Name = "Super Nintendo" };
        var ds = new Platform { Name = "Nintendo DS" };

        // Seed companies
        var square = new Company { Name = "Square" };
        var gameFreak = new Company { Name = "Game Freak" };

        // Seed genres
        var rpg = new Genre { Name = "RPG" };

        // Seed games using navigation properties
        var finalFantasy6 = new Game
        {
            Name = "Final Fantasy VI",
            Developer = square,
            ReleaseDate = new DateTime(1994, 4, 2),
            GamePlatforms = new List<GamePlatform>
            {
                new GamePlatform { Platform = snes }
            },
            GameGenres = new List<GameGenre>
            {
                new GameGenre { Genre = rpg }
            }
        };

        var pokemonPlatinum = new Game
        {
            Name = "Pokémon Platinum",
            Developer = gameFreak,
            ReleaseDate = new DateTime(2008, 9, 13),
            GamePlatforms = new List<GamePlatform>
            {
                new GamePlatform { Platform = ds }
            },
            GameGenres = new List<GameGenre>
            {
                new GameGenre { Genre = rpg }
            }
        };

        dbContext.Platforms.AddRange(snes, ds);
        dbContext.Companies.AddRange(square, gameFreak);
        dbContext.Genres.Add(rpg);

        dbContext.Games.AddRange(finalFantasy6, pokemonPlatinum);

        await dbContext.SaveChangesAsync();
    }
}
