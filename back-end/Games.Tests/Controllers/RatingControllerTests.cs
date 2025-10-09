using System.Net.Http.Json;
using FluentAssertions;
using Games.DTOs;
using Games.Models;

namespace Games.Tests;

public class RatingControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public RatingControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllRatings_ReturnsAllRatings()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/rating");

        response.EnsureSuccessStatusCode();

        var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();

        ratings.Should().NotBeNull();
        ratings.Should().HaveCount(2);
        ratings.Should().Contain(r => r.GameId == 1);
        ratings.Should().Contain(r => r.UserId == 2);
    }

    [Fact]
    public async Task GetRatingsByGameId_ReturnsCorrectRatings()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/rating/game/1");
        var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();

        response.EnsureSuccessStatusCode();
        ratings.Should().NotBeNull();
        ratings.Should().OnlyContain(r => r.GameId == 1);
    }

    [Fact]
    public async Task GetRatingsByUserId_ReturnsCorrectRatings()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/rating/user/1");
        var ratings = await response.Content.ReadFromJsonAsync<List<RatingDto>>();

        response.EnsureSuccessStatusCode();
        ratings.Should().NotBeNull();
        ratings.Should().OnlyContain(r => r.UserId == 1);
    }

    [Fact]
    public async Task Post_CreatesNewRating()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();
        var newRating = new RatingDto
        {
            UserId = 1,
            GameId = 2,
            Rate = 8,
            Title = "Solid Game",
            Description = "Really enjoyable."
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/rating", newRating);

        string responseContent = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created,
            $"Expected Created but got {response.StatusCode}. Response: {responseContent}");

        var createdRating = await response.Content.ReadFromJsonAsync<RatingDto>();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        createdRating.Should().NotBeNull();
        createdRating!.UserId.Should().Be(newRating.UserId);
        createdRating.GameId.Should().Be(newRating.GameId);
        createdRating.Rate.Should().Be(newRating.Rate);
    }
    
    [Fact]
    public async Task Put_UpdatesExistingRating()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();
        var updatedRating = new RatingDto
        {
            UserId = 1,
            GameId = 1,
            Rate = 10,
            Title = "Masterpiece",
            Description = "One of the best games ever."
        };

        HttpResponseMessage response = await client.PutAsJsonAsync("/api/rating/1/1", updatedRating);
        response.EnsureSuccessStatusCode();
        var rating = await response.Content.ReadFromJsonAsync<Rating>();

        response.EnsureSuccessStatusCode();
        rating.Should().NotBeNull();
        rating!.Rate.Should().Be(10);
        rating.Title.Should().Be("Masterpiece");
    }

    [Fact]
    public async Task Delete_RemovesRating()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.DeleteAsync("/api/rating/1/1");

        response.EnsureSuccessStatusCode();
        HttpResponseMessage getResponse = await client.GetAsync("/api/rating/1/1");
        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistentRating_ReturnsNotFound()
    {
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.DeleteAsync("/api/rating/999/999");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}