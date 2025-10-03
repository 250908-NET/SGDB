using FluentAssertions;
using Games.Models;
using Games.Repositories;
using Games.Services;

namespace Games.Tests
{
    public class RatingRepositoryTests : TestBase
    {
        private readonly RatingRepository _repository;

        public RatingRepositoryTests()
        {
            _repository = new RatingRepository(Context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRatings()
        {
            await SeedDataAsync();

            var result = await _repository.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.GameId == 1);
            result.Should().Contain(r => r.UserId == 2);
        }

        [Fact]
        public async Task GetByGameIdAsync_ShouldReturnGameRating()
        {
            await SeedDataAsync();

            var result = await _repository.GetByGameIdAsync(2);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain(r => r.GameId == 2);
            result.Should().Contain(r => r.UserId == 2);
            result.Should().Contain(r => r.Title == "Exactly the same");
            result.Should().Contain(r => r.Rate == 3);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserRatings()
        {
            await SeedDataAsync();

            var result = await _repository.GetByUserIdAsync(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain(r => r.GameId == 1);
            result.Should().Contain(r => r.UserId == 1);
            result.Should().Contain(r => r.Title == "Cant wait for the next installment!");
            result.Should().Contain(r => r.Rate == 9);
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewRating()
        {
            await SeedDataAsync();

            var newRating = new Rating
            {
                GameId = 1,
                UserId = 2,
                Title = "Another review",
                Description = "Adding another rating",
                Rate = 7,
                DateTimeRated = DateTime.UtcNow
            };

            var result = await _repository.AddAsync(newRating);

            result.Should().NotBeNull();

            var gameRatings = await _repository.GetByGameIdAsync(1);
            gameRatings.Should().HaveCount(2);

            gameRatings.Should().Contain(r => r.GameId == 1);
            gameRatings.Should().Contain(r => r.UserId == 2);
            gameRatings.Should().Contain(r => r.Title == "Another review");

            
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyRating()
        {
            await SeedDataAsync();

            var existing = (await _repository.GetAllAsync()).First();
            existing.Title = "Updated Title";
            existing.Rate = 8;

            await _repository.UpdateAsync(existing.UserId, existing.GameId, existing);

            var updated = await _repository.GetByGameIdAsync(existing.GameId);
            updated.Should().Contain(r => r.Title == "Updated Title" && r.Rate == 8);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveRating()
        {
            await SeedDataAsync();

            var existing = (await _repository.GetAllAsync()).First();

            await _repository.DeleteAsync(existing.UserId, existing.GameId);

            var all = await _repository.GetAllAsync();
            all.Should().HaveCount(1);
            all.Should().NotContain(r => r.UserId == existing.UserId && r.GameId == existing.GameId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowIfNotFound()
        {
            await SeedDataAsync();

            Func<Task> act = async () => await _repository.DeleteAsync(999, 999);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Rating not found.");
        }

    }
}