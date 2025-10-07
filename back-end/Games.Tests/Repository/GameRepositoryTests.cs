using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games.Data;
using Games.Models;
using Games.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Games.Tests.Repositories
{
    public class GameRepositoryTests : IDisposable
    {
        private readonly GamesDbContext _context;
        private readonly GameRepository _repository;

        public GameRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GamesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new GamesDbContext(options);
            _repository = new GameRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var dev = new Company { Name = "Dev Studio" };
            var pub = new Company { Name = "Pub Studio" };

            var game1 = new Game
            {
                Name = "Test Game 1",
                ReleaseDate = DateTime.UtcNow.AddYears(-1),
                Developer = dev,
                Publisher = pub
            };

            var game2 = new Game
            {
                Name = "Another Game",
                ReleaseDate = DateTime.UtcNow.AddYears(-2),
                Developer = dev,
                Publisher = pub
            };

            _context.Companies.AddRange(dev, pub);
            _context.Games.AddRange(game1, game2);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, g => Assert.NotNull(g.Developer));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectGame()
        {
            var firstGame = _context.Games.First();
            var result = await _repository.GetByIdAsync(firstGame.GameId);

            Assert.NotNull(result);
            Assert.Equal(firstGame.Name, result!.Name);
        }

        [Fact]
        public async Task GetAllMatchingAsync_ShouldReturnGamesContainingName()
        {
            var result = await _repository.GetAllMatchingAsync("Test");

            Assert.Single(result);
            Assert.Contains("Test", result.First().Name);
        }

        [Fact]
        public async Task GetAllMatchingAsync_ShouldThrow_WhenNameIsNullOrWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetAllMatchingAsync(""));
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetAllMatchingAsync("   "));
        }

        [Fact]
        public async Task AddAsync_ShouldAddGame()
        {
            var dev = _context.Companies.First();
            var pub = _context.Companies.Last();

            var newGame = new Game
            {
                Name = "New Game",
                ReleaseDate = DateTime.UtcNow,
                Developer = dev,
                Publisher = pub
            };

            await _repository.AddAsync(newGame);
            await _repository.SaveChangesAsync();

            var result = await _context.Games.FindAsync(newGame.GameId);
            Assert.NotNull(result);
            Assert.Equal("New Game", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyGame()
        {
            var game = _context.Games.First();
            game.Name = "Updated Game";

            await _repository.UpdateAsync(game);
            await _repository.SaveChangesAsync();

            var updated = await _context.Games.FindAsync(game.GameId);
            Assert.Equal("Updated Game", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveGame()
        {
            var game = _context.Games.First();

            await _repository.DeleteAsync(game.GameId);
            await _repository.SaveChangesAsync();

            var deleted = await _context.Games.FindAsync(game.GameId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task LinkGameToPlatformAsync_ShouldCreateLink()
        {
            var game = _context.Games.First();
            var platform = new Platform { Name = "PC" };
            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();

            await _repository.LinkGameToPlatformAsync(game.GameId, platform.PlatformId);

            var link = await _context.GamePlatforms.FindAsync(game.GameId, platform.PlatformId);
            Assert.NotNull(link);
        }

        [Fact]
        public async Task UpdateGamePlatformAsync_ShouldReplaceLink()
        {
            var game = _context.Games.First();
            var platform1 = new Platform { Name = "Old" };
            var platform2 = new Platform { Name = "New" };

            _context.Platforms.AddRange(platform1, platform2);
            await _context.SaveChangesAsync();

            _context.GamePlatforms.Add(new GamePlatform { GameId = game.GameId, PlatformId = platform1.PlatformId });
            await _context.SaveChangesAsync();

            await _repository.UpdateGamePlatformAsync(game.GameId, platform1.PlatformId, platform2.PlatformId);

            var oldLink = await _context.GamePlatforms.FindAsync(game.GameId, platform1.PlatformId);
            var newLink = await _context.GamePlatforms.FindAsync(game.GameId, platform2.PlatformId);

            Assert.Null(oldLink);
            Assert.NotNull(newLink);
        }

        [Fact]
        public async Task UnlinkGameFromPlatformAsync_ShouldRemoveLink()
        {
            var game = _context.Games.First();
            var platform = new Platform { Name = "Xbox" };
            _context.Platforms.Add(platform);
            _context.GamePlatforms.Add(new GamePlatform { GameId = game.GameId, PlatformId = platform.PlatformId });
            await _context.SaveChangesAsync();

            await _repository.UnlinkGameFromPlatformAsync(game.GameId, platform.PlatformId);

            var link = await _context.GamePlatforms.FindAsync(game.GameId, platform.PlatformId);
            Assert.Null(link);
        }

        [Fact]
        public async Task LinkGameToGenreAsync_ShouldCreateLink()
        {
            var game = _context.Games.First();
            var genre = new Genre { Name = "RPG" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            await _repository.LinkGameToGenreAsync(game.GameId, genre.GenreId);

            var link = await _context.GameGenres.FindAsync(game.GameId, genre.GenreId);
            Assert.NotNull(link);
        }

        [Fact]
        public async Task UpdateGameGenreAsync_ShouldReplaceLink()
        {
            var game = _context.Games.First();
            var genre1 = new Genre { Name = "Old Genre" };
            var genre2 = new Genre { Name = "New Genre" };
            _context.Genres.AddRange(genre1, genre2);
            await _context.SaveChangesAsync();

            _context.GameGenres.Add(new GameGenre { GameId = game.GameId, GenreId = genre1.GenreId });
            await _context.SaveChangesAsync();

            await _repository.UpdateGameGenreAsync(game.GameId, genre1.GenreId, genre2.GenreId);

            var oldLink = await _context.GameGenres.FindAsync(game.GameId, genre1.GenreId);
            var newLink = await _context.GameGenres.FindAsync(game.GameId, genre2.GenreId);

            Assert.Null(oldLink);
            Assert.NotNull(newLink);
        }

        [Fact]
        public async Task UnlinkGameFromGenreAsync_ShouldRemoveLink()
        {
            var game = _context.Games.First();
            var genre = new Genre { Name = "Action" };
            _context.Genres.Add(genre);
            _context.GameGenres.Add(new GameGenre { GameId = game.GameId, GenreId = genre.GenreId });
            await _context.SaveChangesAsync();

            await _repository.UnlinkGameFromGenreAsync(game.GameId, genre.GenreId);

            var link = await _context.GameGenres.FindAsync(game.GameId, genre.GenreId);
            Assert.Null(link);
        }
    }
}
