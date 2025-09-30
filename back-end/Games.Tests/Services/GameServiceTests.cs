using FluentAssertions;
using Moq;
using Games.Models;
using Games.Repositories;
using Games.Services;

namespace Games.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _mockRepository;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _mockRepository = new Mock<IGameRepository>();
            _service = new GameService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            // Arrange
            var expectedGames = new List<Game>
            {
                new Game { GameId = 1, Title = "Spider-Man", Developer = "Insomniac", ReleaseYear = 2020 },
                new Game { GameId = 2, Title = "Halo Infinite", Developer = "343 Industries", ReleaseYear = 2021 }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(expectedGames);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedGames);

            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidIdShouldReturnGame()
        {
            // Arrange
            var expectedGame = new Game
            {
                GameId = 1,
                Title = "Spider-Man",
                Developer = "Insomniac",
                ReleaseYear = 2020
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                           .ReturnsAsync(expectedGame);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedGame);
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidIdShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                           .ReturnsAsync((Game?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddGame()
        {
            // Arrange
            var inputGame = new Game { Title = "Elden Ring", Developer = "FromSoftware", ReleaseYear = 2022 };

            _mockRepository.Setup(repo => repo.AddAsync(inputGame))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(inputGame);

            // Assert
            _mockRepository.Verify(repo => repo.AddAsync(inputGame), Times.Once);
            _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdateAndSaveChanges()
        {
            // Arrange
            var game = new Game
            {
                GameId = 1,
                Title = "Halo",
                Developer = "Bungie",
                ReleaseYear = 2001
            };

            _mockRepository.Setup(r => r.UpdateAsync(game))
                           .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync())
                           .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(game);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(game), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteAndSaveChanges()
        {
            // Arrange
            var gameId = 1;

            _mockRepository.Setup(r => r.DeleteAsync(gameId))
                           .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync())
                           .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(gameId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(gameId), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void ConstructorWithNullRepositoryShouldThrow()
        {
            // Act
            Action act = () => new GameService(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
