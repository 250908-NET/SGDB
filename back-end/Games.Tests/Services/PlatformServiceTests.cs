using FluentAssertions;
using Moq;
using Games.Models;
using Games.Repositories;
using Games.Services;

namespace Games.Tests.Services
{
    public class PlatformServiceTests
    {
        private readonly Mock<IPlatformRepository> _mockRepository;
        private readonly PlatformService _service;

        public PlatformServiceTests()
        {
            _mockRepository = new Mock<IPlatformRepository>();
            _service = new PlatformService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPlatforms()
        {
            // Arrange
            var expectedPlatforms = new List<Platform>
            {
                new Platform { PlatformId = 1, Name = "PlayStation 5", Manufacturer = "Sony", ReleaseYear = 2020 },
                new Platform { PlatformId = 2, Name = "Xbox Series X", Manufacturer = "Microsoft", ReleaseYear = 2020 }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(expectedPlatforms);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedPlatforms);

            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnPlatform()
        {
            // Arrange
            var expectedPlatform = new Platform
            {
                PlatformId = 1,
                Name = "PlayStation 5",
                Manufacturer = "Sony",
                ReleaseYear = 2020
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                           .ReturnsAsync(expectedPlatform);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPlatform);
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                           .ReturnsAsync((Platform?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddPlatform()
        {
            // Arrange
            var inputPlatform = new Platform { Name = "Nintendo Switch", Manufacturer = "Nintendo", ReleaseYear = 2017 };

            _mockRepository.Setup(repo => repo.AddAsync(inputPlatform))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(inputPlatform);

            // Assert
            _mockRepository.Verify(repo => repo.AddAsync(inputPlatform), Times.Once);
            _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var updatedPlatform = new Platform { PlatformId = 1, Name = "Updated Console", Manufacturer = "Updated Manufacturer", ReleaseYear = 2025 };

            _mockRepository.Setup(repo => repo.UpdateAsync(updatedPlatform))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(updatedPlatform);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(updatedPlatform), Times.Once);
            _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePlatformAndSave()
        {
            // Arrange
            int platformId = 1;
            _mockRepository.Setup(repo => repo.DeleteAsync(platformId))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(platformId);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAsync(platformId), Times.Once);
            _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public void Constructor_WithNullRepository_ShouldThrow()
        {
            // Act
            Action act = () => new PlatformService(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        
    }
}
