using FluentAssertions;
using Games.Models;
using Games.Repositories;
using Games.Services;
using Moq;

namespace Games.Tests;

public class RatingServiceTests
{
    private readonly Mock<IRatingRepository> _mockRepository;
    private readonly RatingService _service;

    public RatingServiceTests()
    {
        _mockRepository = new Mock<IRatingRepository>();
        _service = new RatingService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRatings()
    {
        var ratings = new List<Rating>
        {
            new Rating { GameId = 1, UserId = 1, Rate = 5 },
            new Rating { GameId = 2, UserId = 2, Rate = 7 }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(ratings);

        var result = await _service.GetAllAsync();

        result.Should().BeEquivalentTo(ratings);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByGameIdAsync_ShouldReturnRatingsForGame()
    {
        var gameId = 1;
        var ratings = new List<Rating>
        {
            new Rating { GameId = gameId, UserId = 1, Rate = 8 }
        };

        _mockRepository.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync(ratings);

        var result = await _service.GetByGameIdAsync(gameId);

        result.Should().BeEquivalentTo(ratings);
        _mockRepository.Verify(r => r.GetByGameIdAsync(gameId), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnRatingsForUser()
    {
        var userId = 1;
        var ratings = new List<Rating>
        {
            new Rating { GameId = 1, UserId = userId, Rate = 9 }
        };

        _mockRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(ratings);

        var result = await _service.GetByUserIdAsync(userId);

        result.Should().BeEquivalentTo(ratings);
        _mockRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddRating()
    {
        var rating = new Rating { GameId = 1, UserId = 1, Rate = 10 };
        _mockRepository.Setup(r => r.AddAsync(rating)).ReturnsAsync(rating);

        var result = await _service.CreateAsync(rating);

        result.Should().BeEquivalentTo(rating);
        _mockRepository.Verify(r => r.AddAsync(rating), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepositoryUpdate()
    {
        var userId = 2;
        var gameId = 2;
        var rating = new Rating { GameId = gameId, UserId = userId, Rate = 6 };

        _mockRepository.Setup(r => r.UpdateAsync(userId, gameId, rating))
                       .Returns(Task.CompletedTask);

        await _service.UpdateAsync(userId, gameId, rating);

        _mockRepository.Verify(r => r.UpdateAsync(userId, gameId, rating), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete()
    {
        var userId = 1;
        var gameId = 1;

        _mockRepository.Setup(r => r.DeleteAsync(userId, gameId))
                       .Returns(Task.CompletedTask);

        await _service.DeleteAsync(userId, gameId);

        _mockRepository.Verify(r => r.DeleteAsync(userId, gameId), Times.Once);
    }

}
