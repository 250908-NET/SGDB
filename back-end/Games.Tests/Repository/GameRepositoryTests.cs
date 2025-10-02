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

        // FIXME: Check developer field and uncomment
        // [Fact]
        // public async Task GetAllAsync_ShouldReturnAllSeededGames()
        // {
        //     // Arrange
        //     await SeedDataAsync();

        //     // Act
        //     var result = await _repository.GetAllAsync();

        //     // Assert
        //     result.Should().NotBeNull();
        //     result.Should().HaveCount(2);

        //     result.Should().ContainSingle(g => g.Name == "Final Fantasy VI" && g.Developer == "Square");
        //     result.Should().ContainSingle(g => g.Name == "Pokémon Platinum" && g.Developer == "Game Freak");
        // }

        [Fact]
        public async Task GetByIdAsync_WithValidIdShouldReturnGameWithPlatforms()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Final Fantasy VI");
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
                Name = "Chrono Trigger",
                DeveloperId = 1,
                ReleaseDate = new DateTime(1995, 3, 11),
            };

            // Act
            await _repository.AddAsync(newGame);
            await _repository.SaveChangesAsync();

            // Assert
            newGame.GameId.Should().BeGreaterThan(0);

            var gameInDb = await Context.Games.FindAsync(newGame.GameId);
            gameInDb.Should().NotBeNull();
            gameInDb!.Name.Should().Be("Chrono Trigger");
            gameInDb.Developer.Should().Be("Square");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyGameInDatabase()
        {
            // Arrange
            await SeedDataAsync();
            var game = await Context.Games.FirstAsync();
            game.Name = "Final Fantasy III";

            // Act
            await _repository.UpdateAsync(game);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await Context.Games.FindAsync(game.GameId);
            updated!.Name.Should().Be("Final Fantasy III");
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
                Name = "Test Game",
                DeveloperId = 2,
                ReleaseDate = new DateTime(2024, 1, 1),
            };

            await _repository.AddAsync(game);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var gameInDb = await Context.Games.FirstOrDefaultAsync(g => g.Name == "Test Game");
            gameInDb.Should().NotBeNull();
            gameInDb!.Developer.Should().Be("Test Studio");
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldNotPersistIfNotCalled()
        {
            // Arrange
            var game = new Game
            {
                Name = "Uncommitted Game",
                DeveloperId = 2,
                ReleaseDate = new DateTime(2024, 1, 1),
            };

            await _repository.AddAsync(game);
            // Intentionally NOT calling SaveChangesAsync

            // Act
            var gameInDb = await Context.Games.FirstOrDefaultAsync(g => g.Name == "Uncommitted Game");

            // Assert
            gameInDb.Should().BeNull(); // not persisted
        }

    }


}
