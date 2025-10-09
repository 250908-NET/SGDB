using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Games.Controllers;
using Games.Services;
using Games.Models;
using Games.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games.Tests.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<ILogger<GamesController>> _loggerMock;
        private readonly Mock<IGameService> _gameServiceMock;
        private readonly Mock<IPlatformService> _platformServiceMock;
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly Mock<ICompanyService> _companyServiceMock;
        private readonly Mock<IGameImageService> _imageServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _loggerMock = new Mock<ILogger<GamesController>>();
            _gameServiceMock = new Mock<IGameService>();
            _platformServiceMock = new Mock<IPlatformService>();
            _genreServiceMock = new Mock<IGenreService>();
            _companyServiceMock = new Mock<ICompanyService>();
            _imageServiceMock = new Mock<IGameImageService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new GamesController(
                _loggerMock.Object,
                _gameServiceMock.Object,
                _platformServiceMock.Object,
                _genreServiceMock.Object,
                _companyServiceMock.Object,
                _imageServiceMock.Object,
                _mapperMock.Object
            );
        }

        // ---------------------------------------------------
        // GET ALL /api/games
        // ---------------------------------------------------
        [Fact]
        public async Task GetGames_ShouldReturnOk_WithListOfGames()
        {
            var games = new List<Game> { new Game { GameId = 1, Name = "Halo" } };
            var gameDtos = new List<GameDto> { new GameDto { GameId = 1, Name = "Halo" } };

            _gameServiceMock.Setup(s => s.GetGames(null)).ReturnsAsync(games);
            _mapperMock.Setup(m => m.Map<List<GameDto>>(games)).Returns(gameDtos);

            var result = await _controller.GetGames(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<GameDto>>(okResult.Value);
            Assert.Single(value);
        }

        // ---------------------------------------------------
        // GET BY ID /api/games/{id}
        // ---------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_ShouldReturnOk_WhenGameExists()
        {
            var game = new Game { GameId = 1, Name = "Halo" };
            var dto = new GameDto { GameId = 1, Name = "Halo" };

            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(game);
            _mapperMock.Setup(m => m.Map<GameDto>(game)).Returns(dto);

            var result = await _controller.GetByIdAsync(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GameDto>(ok.Value);
            Assert.Equal(1, value.GameId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenGameMissing()
        {
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Game?)null);

            var result = await _controller.GetByIdAsync(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Game not found", notFound.Value);
        }

        // ---------------------------------------------------
        // CREATE /api/games
        // ---------------------------------------------------
        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedResult()
        {
            var createDto = new CreateGameDto
            {
                Name = "New Game",
                PublisherId = 1,
                DeveloperId = 2,
                PlatformIds = new List<int> { 3 },
                GenreIds = new List<int> { 4 }
            };
            var game = new Game { GameId = 1, Name = "New Game" };
            var dto = new GameDto { GameId = 1, Name = "New Game" };

            // Mock company existence
            _companyServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Company { CompanyId = 1 });
            _companyServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new Company { CompanyId = 2 });

            // Mock platform & genre existence
            _platformServiceMock.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(new Platform { PlatformId = 3 });
            _genreServiceMock.Setup(s => s.GetByIdAsync(4)).ReturnsAsync(new Genre { GenreId = 4 });

            _mapperMock.Setup(m => m.Map<Game>(createDto)).Returns(game);
            _mapperMock.Setup(m => m.Map<GameDto>(game)).Returns(dto);

            var result = await _controller.CreateAsync(createDto);

            var created = Assert.IsType<CreatedResult>(result);
            var value = Assert.IsType<GameDto>(created.Value);
            Assert.Equal(1, value.GameId);

            // Verify linking methods called
            _gameServiceMock.Verify(s => s.LinkGameToPlatformAsync(1, 3), Times.Once);
            _gameServiceMock.Verify(s => s.LinkGameToGenreAsync(1, 4), Times.Once);
        }


        // ---------------------------------------------------
        // UPDATE /api/games/{id}
        // ---------------------------------------------------
        [Fact]
        public async Task UpdateAsync_ShouldReturnOk_WhenSuccessful()
        {
            var existing = new Game { GameId = 1, Name = "Old Name" };
            var dto = new UpdateGameDto
            {
                Name = "New Name",
                PublisherId = 1,
                DeveloperId = 2,
                PlatformIds = new List<int> { 3 },
                GenreIds = new List<int> { 4 }
            };
            var mappedDto = new GameDto { GameId = 1, Name = "New Name" };

            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(existing);

            _companyServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Company { CompanyId = 1 });
            _companyServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new Company { CompanyId = 2 });
            _platformServiceMock.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(new Platform { PlatformId = 3 });
            _genreServiceMock.Setup(s => s.GetByIdAsync(4)).ReturnsAsync(new Genre { GenreId = 4 });

            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map<GameDto>(existing)).Returns(mappedDto);

            var result = await _controller.UpdateAsync(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GameDto>(ok.Value);
            Assert.Equal("New Name", value.Name);

            _gameServiceMock.Verify(s => s.ClearGamePlatformsAsync(1), Times.Once);
            _gameServiceMock.Verify(s => s.ClearGameGenresAsync(1), Times.Once);
            _gameServiceMock.Verify(s => s.LinkGameToPlatformAsync(1, 3), Times.Once);
            _gameServiceMock.Verify(s => s.LinkGameToGenreAsync(1, 4), Times.Once);
        }


        [Fact]
        public async Task UpdateAsync_ShouldReturnNotFound_WhenGameMissing()
        {
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Game?)null);
            var result = await _controller.UpdateAsync(1, new UpdateGameDto());
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ---------------------------------------------------
        // DELETE /api/games/{id}
        // ---------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldReturnNoContent_WhenSuccessful()
        {
            var game = new Game { GameId = 1 };
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(game);

            var result = await _controller.DeleteAsync(1);

            Assert.IsType<NoContentResult>(result);
            _gameServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNotFound_WhenGameMissing()
        {
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Game?)null);

            var result = await _controller.DeleteAsync(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Game not found", notFound.Value);
        }

        [Fact]
        public async Task LinkGameToPlatform_ShouldReturnNotFound_WhenGameMissing()
        {
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Game?)null);

            var result = await _controller.LinkGameToPlatform(1, 2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Game with ID 1", notFound.Value!.ToString());
        }

        [Fact]
        public async Task LinkGameToPlatform_ShouldReturnNotFound_WhenPlatformMissing()
        {
            var game = new Game { GameId = 1 };
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(game);
            _platformServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync((Platform?)null);

            var result = await _controller.LinkGameToPlatform(1, 2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Platform with ID 2", notFound.Value!.ToString());
        }

        [Fact]
        public async Task LinkGameToPlatform_ShouldReturnBadRequest_WhenAlreadyLinked()
        {
            var game = new Game
            {
                GameId = 1,
                GamePlatforms = new List<GamePlatform> { new GamePlatform { PlatformId = 2 } }
            };

            var platform = new Platform { PlatformId = 2 };
            _gameServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(game);
            _platformServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(platform);

            var result = await _controller.LinkGameToPlatform(1, 2);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This game is already linked to that platform.", bad.Value);
        }
    }
}
