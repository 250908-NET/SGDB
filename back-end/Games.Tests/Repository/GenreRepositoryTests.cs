using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games.Data;
using Games.Models;
using Games.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Games.Tests.Repositories
{
    public class GenreRepositoryTests
    {
        private readonly DbContextOptions<GamesDbContext> _options;

        public GenreRepositoryTests()
        {
            // Use a unique in-memory database for each test run
            _options = new DbContextOptionsBuilder<GamesDbContext>()
                .UseInMemoryDatabase(databaseName: $"GamesDb_{System.Guid.NewGuid()}")
                .Options;
        }

        private static Genre CreateGenre(string name = "Action")
        {
            return new Genre { Name = name };
        }

        [Fact]
        public async Task AddAsync_ShouldAddGenreToDatabase()
        {
            using var context = new GamesDbContext(_options);
            var repo = new GenreRepository(context);
            var genre = CreateGenre("RPG");

            await repo.AddAsync(genre);
            await repo.SaveChangesAsync();

            Assert.Equal(1, context.Genres.Count());
            Assert.Equal("RPG", context.Genres.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGenres()
        {
            using var context = new GamesDbContext(_options);
            context.Genres.AddRange(
                CreateGenre("Action"),
                CreateGenre("Adventure")
            );
            await context.SaveChangesAsync();

            var repo = new GenreRepository(context);
            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.Name == "Action");
            Assert.Contains(result, g => g.Name == "Adventure");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGenre_WhenExists()
        {
            using var context = new GamesDbContext(_options);
            var genre = CreateGenre("Strategy");
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var repo = new GenreRepository(context);
            var result = await repo.GetByIdAsync(genre.GenreId);

            Assert.NotNull(result);
            Assert.Equal("Strategy", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            using var context = new GamesDbContext(_options);
            var repo = new GenreRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingGenre()
        {
            using var context = new GamesDbContext(_options);
            var genre = CreateGenre("Old Name");
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var repo = new GenreRepository(context);
            genre.Name = "New Name";
            await repo.UpdateAsync(genre);
            await repo.SaveChangesAsync();

            var updated = await context.Genres.FindAsync(genre.GenreId);
            Assert.Equal("New Name", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveGenre_WhenExists()
        {
            using var context = new GamesDbContext(_options);
            var genre = CreateGenre("Horror");
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var repo = new GenreRepository(context);
            await repo.DeleteAsync(genre.GenreId);
            await repo.SaveChangesAsync();

            Assert.Empty(context.Genres);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenNotFound()
        {
            using var context = new GamesDbContext(_options);
            var repo = new GenreRepository(context);

            await repo.DeleteAsync(999); // does not exist
            await repo.SaveChangesAsync();

            Assert.Empty(context.Genres);
        }
    }
}
