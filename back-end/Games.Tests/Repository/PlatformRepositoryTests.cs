using FluentAssertions;
using Games.Data;
using Games.Models;
using Games.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Games.Tests
{
    public class PlatformRepositoryTests : TestBase
    {
        private readonly PlatformRepository _repository;

        public PlatformRepositoryTests()
        {
            _repository = new PlatformRepository(Context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllSeededPlatforms()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result.Should().ContainSingle(p => p.Name == "Super Nintendo");
            result.Should().ContainSingle(p => p.Name == "Nintendo DS");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnPlatformWithGames()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Super Nintendo");
            result.GamePlatforms.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldAddPlatformToDatabase()
        {
            // Arrange
            await SeedDataAsync();

            var newPlatform = new Platform
            {
                Name = "Nintendo Switch",
            };

            // Act
            await _repository.AddAsync(newPlatform);
            await _repository.SaveChangesAsync();

            // Assert
            newPlatform.PlatformId.Should().BeGreaterThan(0);

            var platformInDb = await Context.Platforms.FindAsync(newPlatform.PlatformId);
            platformInDb.Should().NotBeNull();
            platformInDb!.Name.Should().Be("Nintendo Switch");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyPlatformInDatabase()
        {
            // Arrange
            await SeedDataAsync();
            var platform = await Context.Platforms.FirstAsync();
            platform.Name = "SNES";

            // Act
            await _repository.UpdateAsync(platform);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await Context.Platforms.FindAsync(platform.PlatformId);
            updated!.Name.Should().Be("SNES");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePlatformFromDatabase()
        {
            // Arrange
            await SeedDataAsync();
            var platform = await Context.Platforms.FirstAsync();

            // Act
            await _repository.DeleteAsync(platform.PlatformId);
            await _repository.SaveChangesAsync();

            // Assert
            var deleted = await Context.Platforms.FindAsync(platform.PlatformId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChangesToDatabase()
        {
            // Arrange
            var platform = new Platform
            {
                Name = "Test Console",
            };

            await _repository.AddAsync(platform);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var platformInDb = await Context.Platforms.FirstOrDefaultAsync(p => p.Name == "Test Console");
            platformInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldNotPersistIfNotCalled()
        {
            // Arrange
            var platform = new Platform
            {
                Name = "Uncommitted Console",
            };

            await _repository.AddAsync(platform);
            // Intentionally NOT calling SaveChangesAsync

            // Act
            var platformInDb = await Context.Platforms.FirstOrDefaultAsync(p => p.Name == "Uncommitted Console");

            // Assert
            platformInDb.Should().BeNull(); // not persisted
        }
    }
}
