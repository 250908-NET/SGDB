using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Controllers;
using Games.Services;
using Games.DTOs;
using Games.Models;
using System;

namespace Games.Tests.Controllers
{
    public class GenresControllerTests
    {
        private readonly Mock<ILogger<GenresController>> _mockLogger;
        private readonly Mock<IGenreService> _mockService;
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GenresController _controller;

        public GenresControllerTests()
        {
            _mockLogger = new Mock<ILogger<GenresController>>();
            _mockService = new Mock<IGenreService>();
            _mockGameService = new Mock<IGameService>();
            _mockMapper = new Mock<IMapper>();

            _controller = new GenresController(
                _mockLogger.Object,
                _mockService.Object,
                _mockGameService.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOk_WithMappedGenres()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Action" },
                new Genre { GenreId = 2, Name = "Adventure" }
            };
            var genreDtos = new List<GenreDto>
            {
                new GenreDto { GenreId = 1, Name = "Action" },
                new GenreDto { GenreId = 2, Name = "Adventure" }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(genres);
            _mockMapper.Setup(m => m.Map<List<GenreDto>>(genres)).Returns(genreDtos);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GenreDto>>(okResult.Value);
            Assert.Equal(2, ((List<GenreDto>)returnValue).Count);
        }

        [Fact]
        public async Task GetGenre_ShouldReturnOk_WhenGenreExists()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "RPG" };
            var dto = new GenreDto { GenreId = 1, Name = "RPG" };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(genre);
            _mockMapper.Setup(m => m.Map<GenreDto>(genre)).Returns(dto);

            // Act
            var result = await _controller.GetGenre(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<GenreDto>(okResult.Value);
            Assert.Equal("RPG", returnValue.Name);
        }

        [Fact]
        public async Task GetGenre_ShouldReturnNotFound_WhenGenreMissing()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Genre?)null);

            // Act
            var result = await _controller.GetGenre(99);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Genre not found", notFound.Value);
        }

        [Fact]
        public async Task CreateGenre_ShouldReturnCreated_WithGenreDto()
        {
            // Arrange
            var createDto = new CreateGenreDto
            {
                Name = "Horror",
                Games = new List<int> { 1 }
            };

            var genre = new Genre { GenreId = 1, Name = "Horror" };
            var dto = new GenreDto { GenreId = 1, Name = "Horror" };

            _mockMapper.Setup(m => m.Map<Genre>(createDto)).Returns(genre);
            _mockMapper.Setup(m => m.Map<GenreDto>(genre)).Returns(dto);
            _mockService.Setup(s => s.CreateAsync(It.IsAny<Genre>())).Returns(Task.CompletedTask);
            _mockGameService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Game { GameId = 1 });
            _mockGameService.Setup(s => s.LinkGameToGenreAsync(1, 1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateGenre(createDto);

            // Assert
            var created = Assert.IsType<CreatedResult>(result.Result);
            var returnValue = Assert.IsType<GenreDto>(created.Value);
            Assert.Equal("Horror", returnValue.Name);
            _mockService.Verify(s => s.CreateAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGenre_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var existing = new Genre { GenreId = 1, Name = "Old" };
            var dto = new UpdateGenreDto { Name = "Updated", Games = new List<int> { 1 } };
            var updated = new GenreDto { GenreId = 1, Name = "Updated" };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockService.Setup(s => s.ClearGenreGamesAsync(1)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map(dto, existing));
            _mockService.Setup(s => s.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<GenreDto>(existing)).Returns(updated);
            _mockGameService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Game { GameId = 1 });
            _mockGameService.Setup(s => s.LinkGameToGenreAsync(1, 1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateGenre(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenreDto>(okResult.Value);
            Assert.Equal("Updated", returnValue.Name);
        }

        [Fact]
        public async Task UpdateGenre_ShouldReturnNotFound_WhenGenreMissing()
        {
            // Arrange
            var dto = new UpdateGenreDto { Name = "Updated", Games = new List<int>() };
            _mockService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((Genre?)null);

            // Act
            var result = await _controller.UpdateGenre(5, dto);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Genre not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteGenre_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var existing = new Genre { GenreId = 1, Name = "Action" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockService.Setup(s => s.ClearGenreGamesAsync(1)).Returns(Task.CompletedTask);
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGenre(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteGenre_ShouldReturnNotFound_WhenGenreMissing()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((Genre?)null);

            // Act
            var result = await _controller.DeleteGenre(9);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Genre not found", notFound.Value);
        }
    }
}
