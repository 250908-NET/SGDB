using System.Reflection.Metadata;
using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

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
        dbContext.Ratings.RemoveRange(dbContext.Ratings);
        dbContext.Users.RemoveRange(dbContext.Users);
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

        // Seed users - Needs to be changed
        var user1 = new User
        {
            username = "testuser",
            role = "User"
        };
        var user2 = new User()
        {
            username = "testadmin",
            role = "Admin"
        };

        //Seed Ratings
        var rate1 = new Rating
        {
            Game = finalFantasy6,
            User = user1,
            Title = "Cant wait for the next installment!",
            Description = "Feels like the next game will keep this franchise from ever being forgotten!",
            Rate = 9,
            DateTimeRated = DateTime.UtcNow
        };

        var rate2 = new Rating
        {
            Game = pokemonPlatinum,
            User = user2,
            Title = "Exactly the same",
            Description = "Feels like they are never trying anything new.",
            Rate = 3,
            DateTimeRated = DateTime.UtcNow
        };

        dbContext.Platforms.AddRange(snes, ds);
        dbContext.Companies.AddRange(square, gameFreak);
        dbContext.Genres.Add(rpg);

        dbContext.Games.AddRange(finalFantasy6, pokemonPlatinum);
        
        dbContext.Users.AddRange(user1, user2);
        dbContext.Ratings.AddRange(rate1, rate2);

        await dbContext.SaveChangesAsync();
    }
}
