using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Models;
using Games.Repositories;
using Games.Services;
using System;

namespace Games.Tests.Services
{
    public class GenreServiceTests
    {
        private readonly Mock<IGenreRepository> _mockRepo;
        private readonly GenreService _service;

        public GenreServiceTests()
        {
            _mockRepo = new Mock<IGenreRepository>();
            _service = new GenreService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnGenres()
        {
            // Arrange
            var genres = new List<Genre> 
            {
                new Genre { GenreId = 1, Name = "Action" },
                new Genre { GenreId = 2, Name = "Adventure" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.Name == "Action");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGenre_WhenFound()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Action" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Action", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Genre?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSaveGenre()
        {
            // Arrange
            var genre = new Genre { Name = "RPG" };

            // Act
            await _service.CreateAsync(genre);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(genre), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSaveGenre()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Strategy" };

            // Act
            await _service.UpdateAsync(genre);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(genre), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAndSaveGenre()
        {
            // Arrange
            int id = 1;

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepoIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GenreService(null!));
        }
    }
}
