using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Models;
using Games.Repositories;
using Games.Services;
using Moq;
using Xunit;

namespace Games.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _repoMock;
        private readonly Mock<IGameImageService> _imageServiceMock;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _repoMock = new Mock<IGameRepository>();
            _imageServiceMock = new Mock<IGameImageService>();
            _service = new GameService(_repoMock.Object, _imageServiceMock.Object);
        }

        [Fact]
        public async Task GetGames_ShouldReturnAll_WhenNameIsNull()
        {
            // Arrange
            var games = new List<Game> { new Game { GameId = 1, Name = "Halo" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

            // Act
            var result = await _service.GetGames(null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Halo", result[0].Name);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
            _repoMock.Verify(r => r.GetAllMatchingAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetGames_ShouldReturnMatching_WhenNameIsNotNull()
        {
            var games = new List<Game> { new Game { GameId = 2, Name = "Zelda" } };
            _repoMock.Setup(r => r.GetAllMatchingAsync("Zelda")).ReturnsAsync(games);

            var result = await _service.GetGames("Zelda");

            Assert.Single(result);
            Assert.Equal(2, result[0].GameId);
            _repoMock.Verify(r => r.GetAllMatchingAsync("Zelda"), Times.Once);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGame()
        {
            var game = new Game { GameId = 1, Name = "Mario" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);

            var result = await _service.GetByIdAsync(1);

            Assert.Equal("Mario", result!.Name);
            _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSave()
        {
            var game = new Game { GameId = 3, Name = "Metroid" };
            _imageServiceMock.Setup(s => s.GetGameImageUrlAsync("Metroid"))
                .ReturnsAsync("http://example.com/metroid.jpg");

            await _service.CreateAsync(game);
            
            Assert.Equal("http://example.com/metroid.jpg", game.ImageUrl);
            _repoMock.Verify(r => r.AddAsync(game), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSave()
        {
            var game = new Game { GameId = 4, Name = "Doom" };
             _imageServiceMock.Setup(s => s.GetGameImageUrlAsync("Doom"))
                .ReturnsAsync("http://example.com/doom.jpg");

            await _service.UpdateAsync(game);

            Assert.Equal("http://example.com/doom.jpg", game.ImageUrl);
            _repoMock.Verify(r => r.UpdateAsync(game), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSave()
        {
            await _service.DeleteAsync(5);

            _repoMock.Verify(r => r.DeleteAsync(5), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LinkGameToPlatformAsync_ShouldCallRepo()
        {
            await _service.LinkGameToPlatformAsync(1, 2);
            _repoMock.Verify(r => r.LinkGameToPlatformAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task UpdateGamePlatformAsync_ShouldCallRepo()
        {
            await _service.UpdateGamePlatformAsync(1, 10, 20);
            _repoMock.Verify(r => r.UpdateGamePlatformAsync(1, 10, 20), Times.Once);
        }

        [Fact]
        public async Task UnlinkGameFromPlatformAsync_ShouldCallRepo()
        {
            await _service.UnlinkGameFromPlatformAsync(1, 2);
            _repoMock.Verify(r => r.UnlinkGameFromPlatformAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task LinkGameToGenreAsync_ShouldCallRepo()
        {
            await _service.LinkGameToGenreAsync(1, 99);
            _repoMock.Verify(r => r.LinkGameToGenreAsync(1, 99), Times.Once);
        }

        [Fact]
        public async Task UpdateGameGenreAsync_ShouldCallRepo()
        {
            await _service.UpdateGameGenreAsync(1, 5, 6);
            _repoMock.Verify(r => r.UpdateGameGenreAsync(1, 5, 6), Times.Once);
        }

        [Fact]
        public async Task UnlinkGameFromGenreAsync_ShouldCallRepo()
        {
            await _service.UnlinkGameFromGenreAsync(1, 77);
            _repoMock.Verify(r => r.UnlinkGameFromGenreAsync(1, 77), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenRepoIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GameService(null!, _imageServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new GameService(_repoMock.Object, null!));
        }
    }
}
