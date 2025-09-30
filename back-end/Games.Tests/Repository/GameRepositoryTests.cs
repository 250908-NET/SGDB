using FluentAssertions;
using Games.Data;
using Games.Models;
using Games.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Games.Tests
{
    public class GameRepositoryTests : TestBase
    {
        private readonly GameRepository _repository;

        public GameRepositoryTests()
        {
            _repository = new GameRepository(Context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllSeededGames()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result.Should().ContainSingle(g => g.Title == "Final Fantasy VI" && g.Developer == "Square");
            result.Should().ContainSingle(g => g.Title == "Pokémon Platinum" && g.Developer == "Game Freak");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidIdShouldReturnGameWithPlatforms()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Final Fantasy VI");
            result.Developer.Should().Be("Square");
            result.GamePlatforms.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidIdShouldReturnNull()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldAddGameToDatabase()
        {
            // Arrange
            await SeedDataAsync();

            var newGame = new Game
            {
                Title = "Chrono Trigger",
                Developer = "Square",
                ReleaseYear = 1995
            };

            // Act
            await _repository.AddAsync(newGame);
            await _repository.SaveChangesAsync();

            // Assert
            newGame.GameId.Should().BeGreaterThan(0);

            var gameInDb = await Context.Games.FindAsync(newGame.GameId);
            gameInDb.Should().NotBeNull();
            gameInDb!.Title.Should().Be("Chrono Trigger");
            gameInDb.Developer.Should().Be("Square");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyGameInDatabase()
        {
            // Arrange
            await SeedDataAsync();
            var game = await Context.Games.FirstAsync();
            game.Title = "Final Fantasy III";

            // Act
            await _repository.UpdateAsync(game);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await Context.Games.FindAsync(game.GameId);
            updated!.Title.Should().Be("Final Fantasy III");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveGameFromDatabase()
        {
            // Arrange
            await SeedDataAsync();
            var game = await Context.Games.FirstAsync();

            // Act
            await _repository.DeleteAsync(game.GameId);
            await _repository.SaveChangesAsync();

            // Assert
            var deleted = await Context.Games.FindAsync(game.GameId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChangesToDatabase()
        {
            // Arrange
            var game = new Game
            {
                Title = "Test Game",
                Developer = "Test Studio",
                ReleaseYear = 2024
            };

            await _repository.AddAsync(game);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var gameInDb = await Context.Games.FirstOrDefaultAsync(g => g.Title == "Test Game");
            gameInDb.Should().NotBeNull();
            gameInDb!.Developer.Should().Be("Test Studio");
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldNotPersistIfNotCalled()
        {
            // Arrange
            var game = new Game
            {
                Title = "Uncommitted Game",
                Developer = "Test Studio",
                ReleaseYear = 2024
            };

            await _repository.AddAsync(game);
            // Intentionally NOT calling SaveChangesAsync

            // Act
            var gameInDb = await Context.Games.FirstOrDefaultAsync(g => g.Title == "Uncommitted Game");

            // Assert
            gameInDb.Should().BeNull(); // not persisted
        }
        
    }
    

}
