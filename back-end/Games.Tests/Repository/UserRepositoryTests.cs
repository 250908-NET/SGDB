using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Games.Data;
using Games.Models;
using Games.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Games.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly GamesDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserRepository _repo;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GamesDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new GamesDbContext(options);

            var config = new MapperConfiguration(cfg => { });
            _mapper = config.CreateMapper();

            _repo = new UserRepository(_context, _mapper);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User { username = "testuser", role = "player" };

            // Act
            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.Single(users);
            Assert.Equal("testuser", users[0].username);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            _context.Users.AddRange(
                new User { username = "user1", role = "player" },
                new User { username = "user2", role = "admin" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repo.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.username == "user1");
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User { username = "userX", role = "player" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var found = await _repo.GetUserByIDAsync(user.UserId);

            // Assert
            Assert.NotNull(found);
            Assert.Equal("userX", found!.username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var user = new User { username = "CaseTest", role = "player" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var found = await _repo.GetUserByUsernameAsync("casetest");

            // Assert
            Assert.NotNull(found);
            Assert.Equal("CaseTest", found!.username);
        }

        [Fact]
        public async Task ChangeUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = new User { username = "oldname", role = "player" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.username = "newname";
            await _repo.ChangeUserAsync(user);
            await _repo.SaveChangesAsync();

            // Assert
            var updated = await _context.Users.FirstAsync();
            Assert.Equal("newname", updated.username);
        }

        [Fact]
        public async Task RemoveUserAsync_ShouldRemoveUser()
        {
            // Arrange
            var user = new User { username = "deleteme", role = "player" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _repo.RemoveUserAsync(user.UserId);
            await _repo.SaveChangesAsync();

            // Assert
            var count = await _context.Users.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task LinkUserToGenreAsync_ShouldAddLink_WhenNotExisting()
        {
            // Arrange
            var user = new User { username = "genrelink", role = "player" };
            var genre = new Genre { Name = "RPG" };
            _context.Users.Add(user);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Act
            await _repo.LinkUserToGenreAsync(user.UserId, genre.GenreId);

            // Assert
            var link = await _context.UserGenres.FindAsync(user.UserId, genre.GenreId);
            Assert.NotNull(link);
        }

        [Fact]
        public async Task UnlinkUserFromGenreAsync_ShouldRemoveExistingLink()
        {
            // Arrange
            var user = new User { username = "unlinker", role = "player" };
            var genre = new Genre { Name = "Action" };
            _context.Users.Add(user);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            _context.UserGenres.Add(new UserGenre { UserId = user.UserId, GenreId = genre.GenreId });
            await _context.SaveChangesAsync();

            // Act
            await _repo.UnlinkUserFromGenreAsync(user.UserId, genre.GenreId);

            // Assert
            var link = await _context.UserGenres.FindAsync(user.UserId, genre.GenreId);
            Assert.Null(link);
        }

        [Fact]
        public async Task LinkUserToGameAsync_ShouldAddLink_WhenNotExisting()
        {
            // Arrange
            var user = new User { username = "gameuser", role = "player" };
            var game = new Game { Name = "Half-Life" };
            _context.Users.Add(user);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Act
            await _repo.LinkUserToGameAsync(user.UserId, game.GameId);

            // Assert
            var link = await _context.UserGames.FindAsync(user.UserId, game.GameId);
            Assert.NotNull(link);
        }

        [Fact]
        public async Task UnlinkUserFromGameAsync_ShouldRemoveExistingLink()
        {
            // Arrange
            var user = new User { username = "unlinkgame", role = "player" };
            var game = new Game { Name = "Portal" };
            _context.Users.Add(user);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            _context.UserGames.Add(new UserGame { UserId = user.UserId, GameId = game.GameId });
            await _context.SaveChangesAsync();

            // Act
            await _repo.UnlinkUserFromGameAsync(user.UserId, game.GameId);

            // Assert
            var link = await _context.UserGames.FindAsync(user.UserId, game.GameId);
            Assert.Null(link);
        }
    }
}
