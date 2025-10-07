using Xunit;
using Moq;
using Games.Services;
using Games.Repositories;
using Games.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { UserId = 1, username = "Alice", role = "player" },
                new() { UserId = 2, username = "Bob", role = "admin" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].username);
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = new User { UserId = 1, username = "Alice", role = "player" };
            _mockRepo.Setup(r => r.GetUserByIDAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result!.username);
            _mockRepo.Verify(r => r.GetUserByIDAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetUserByIDAsync(5)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByIdAsync(5);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldCallRepo_WithLowercaseUsername()
        {
            // Arrange
            var user = new User { UserId = 1, username = "TestUser", role = "player" };
            _mockRepo.Setup(r => r.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByUsernameAsync("TestUser");

            // Assert
            Assert.NotNull(result);
            _mockRepo.Verify(r => r.GetUserByUsernameAsync("testuser"), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldCallRepoAddAndSave()
        {
            // Arrange
            var user = new User { username = "NewGuy", role = "player" };

            // Act
            await _service.AddUserAsync(user);

            // Assert
            _mockRepo.Verify(r => r.AddUserAsync(user), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangeUserAsync_ShouldCallRepoUpdateAndSave()
        {
            // Arrange
            var user = new User { UserId = 1, username = "EditMe" };

            // Act
            await _service.ChangeUserAsync(user);

            // Assert
            _mockRepo.Verify(r => r.ChangeUserAsync(user), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_ShouldCallRepoRemoveAndSave()
        {
            // Arrange
            int id = 1;

            // Act
            await _service.RemoveUserAsync(id);

            // Assert
            _mockRepo.Verify(r => r.RemoveUserAsync(id), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LinkUserToGenreAsync_ShouldCallRepo()
        {
            // Arrange
            int userId = 1, genreId = 10;

            // Act
            await _service.LinkUserToGenreAsync(userId, genreId);

            // Assert
            _mockRepo.Verify(r => r.LinkUserToGenreAsync(userId, genreId), Times.Once);
        }

        [Fact]
        public async Task UnlinkUserFromGenreAsync_ShouldCallRepo()
        {
            int userId = 1, genreId = 10;

            await _service.UnlinkUserFromGenreAsync(userId, genreId);

            _mockRepo.Verify(r => r.UnlinkUserFromGenreAsync(userId, genreId), Times.Once);
        }

        [Fact]
        public async Task LinkUserToGameAsync_ShouldCallRepo()
        {
            int userId = 2, gameId = 5;

            await _service.LinkUserToGameAsync(userId, gameId);

            _mockRepo.Verify(r => r.LinkUserToGameAsync(userId, gameId), Times.Once);
        }

        [Fact]
        public async Task UnlinkUserFromGameAsync_ShouldCallRepo()
        {
            int userId = 3, gameId = 9;

            await _service.UnlinkUserFromGameAsync(userId, gameId);

            _mockRepo.Verify(r => r.UnlinkUserFromGameAsync(userId, gameId), Times.Once);
        }
    }
}
