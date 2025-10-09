using Xunit;
using Moq;
using Games.Services;
using Games.Repositories;
using Games.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Games.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IGenreRepository> _mockGenreRepo;
        private readonly Mock<IGameRepository> _mockGameRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockGenreRepo = new Mock<IGenreRepository>();
            _mockGameRepo = new Mock<IGameRepository>();
            _service = new UserService(_mockUserRepo.Object, _mockGenreRepo.Object, _mockGameRepo.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            var users = new List<User>
            {
                new() { UserId = 1, username = "Alice", role = "player" },
                new() { UserId = 2, username = "Bob", role = "admin" }
            };
            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].username);
            _mockUserRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { UserId = 1, username = "Alice", role = "player" };
            _mockUserRepo.Setup(r => r.GetUserByIDAsync(1)).ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Alice", result!.username);
            _mockUserRepo.Verify(r => r.GetUserByIDAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockUserRepo.Setup(r => r.GetUserByIDAsync(5)).ReturnsAsync((User?)null);

            var result = await _service.GetUserByIdAsync(5);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldCallRepo_WithLowercaseUsername()
        {
            var user = new User { UserId = 1, username = "TestUser", role = "player" };
            _mockUserRepo.Setup(r => r.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

            var result = await _service.GetUserByUsernameAsync("TestUser");

            Assert.NotNull(result);
            _mockUserRepo.Verify(r => r.GetUserByUsernameAsync("testuser"), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldCallRepoAddAndSave_WhenGenresAndGamesExist()
        {
            var user = new User
            {
                username = "NewGuy",
                role = "player",
                UserGenres = new List<UserGenre> { new() { GenreId = 1 } },
                GameLibrary = new List<UserGame> { new() { GameId = 10 } }
            };

            _mockGenreRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Genre { GenreId = 1 });
            _mockGameRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Game { GameId = 10 });

            await _service.AddUserAsync(user);

            _mockUserRepo.Verify(r => r.AddUserAsync(user), Times.Once);
            _mockUserRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangeUserAsync_ShouldCallRepoUpdateAndSave_WhenUserExists()
        {
            var user = new User
            {
                UserId = 1,
                username = "EditMe",
                role = "player",
                UserGenres = new List<UserGenre> { new() { GenreId = 2 } },
                GameLibrary = new List<UserGame> { new() { GameId = 5 } }
            };

            _mockUserRepo.Setup(r => r.GetUserByIDAsync(1)).ReturnsAsync(new User { UserId = 1 });
            _mockGenreRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Genre { GenreId = 2 });
            _mockGameRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Game { GameId = 5 });

            await _service.ChangeUserAsync(user);

            _mockUserRepo.Verify(r => r.ChangeUserAsync(user), Times.Once);
            _mockUserRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_ShouldCallRepoRemoveAndSave_WhenUserExists()
        {
            int id = 1;
            _mockUserRepo.Setup(r => r.GetUserByIDAsync(id)).ReturnsAsync(new User { UserId = id });

            await _service.RemoveUserAsync(id);

            _mockUserRepo.Verify(r => r.RemoveUserAsync(id), Times.Once);
            _mockUserRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LinkUserToGenreAsync_ShouldLink_WhenValid()
        {
            int userId = 1, genreId = 10;
            var user = new User { UserId = userId, UserGenres = new List<UserGenre>() };

            _mockUserRepo.Setup(r => r.GetUserByIDAsync(userId)).ReturnsAsync(user);
            _mockGenreRepo.Setup(r => r.GetByIdAsync(genreId)).ReturnsAsync(new Genre { GenreId = genreId });

            await _service.LinkUserToGenreAsync(userId, genreId);

            _mockUserRepo.Verify(r => r.LinkUserToGenreAsync(userId, genreId), Times.Once);
        }

        [Fact]
        public async Task UnlinkUserFromGenreAsync_ShouldUnlink_WhenLinked()
        {
            int userId = 1, genreId = 10;
            var user = new User
            {
                UserId = userId,
                UserGenres = new List<UserGenre> { new() { GenreId = genreId } }
            };

            _mockUserRepo.Setup(r => r.GetUserByIDAsync(userId)).ReturnsAsync(user);

            await _service.UnlinkUserFromGenreAsync(userId, genreId);

            _mockUserRepo.Verify(r => r.UnlinkUserFromGenreAsync(userId, genreId), Times.Once);
        }

        [Fact]
        public async Task LinkUserToGameAsync_ShouldLink_WhenValid()
        {
            int userId = 2, gameId = 5;
            var user = new User { UserId = userId, GameLibrary = new List<UserGame>() };

            _mockUserRepo.Setup(r => r.GetUserByIDAsync(userId)).ReturnsAsync(user);
            _mockGameRepo.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(new Game { GameId = gameId });

            await _service.LinkUserToGameAsync(userId, gameId);

            _mockUserRepo.Verify(r => r.LinkUserToGameAsync(userId, gameId), Times.Once);
        }

        [Fact]
        public async Task UnlinkUserFromGameAsync_ShouldCallRepo()
        {
            int userId = 3, gameId = 9;

            await _service.UnlinkUserFromGameAsync(userId, gameId);

            _mockUserRepo.Verify(r => r.UnlinkUserFromGameAsync(userId, gameId), Times.Once);
        }
    }
}
