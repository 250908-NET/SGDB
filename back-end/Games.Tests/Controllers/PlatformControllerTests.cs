using System.Net.Http.Json;
using FluentAssertions;
using Games.DTOs;
using Games.Models;

namespace Games.Tests;

public class PlatformControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public PlatformControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ReturnsSuccessAndCorrectContent()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/platforms");
        List<Platform>? platforms = await response.Content.ReadFromJsonAsync<List<Platform>>();

        // Assert
        response.EnsureSuccessStatusCode();
        platforms.Should().NotBeNull();
        platforms.Should().HaveCount(2);
        platforms.Should().ContainSingle(p => p.Name == "Super Nintendo");
        platforms.Should().ContainSingle(p => p.Name == "Nintendo DS");
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsPlatform()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/platforms/1");
        Platform? platform = await response.Content.ReadFromJsonAsync<Platform>();

        // Assert
        response.EnsureSuccessStatusCode();
        platform.Should().NotBeNull();
        platform.Name.Should().Be("Super Nintendo");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/platforms/999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CreatesNewPlatform()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();
        var newPlatform = new CreatePlatformDto
        {
            Name = "PlayStation 5",
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/platforms", newPlatform);
        Platform? createdPlatform = await response.Content.ReadFromJsonAsync<Platform>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        createdPlatform.Should().NotBeNull();
        createdPlatform!.PlatformId.Should().BeGreaterThan(0);
        createdPlatform.Name.Should().Be(newPlatform.Name);
    }

    [Fact]
    public async Task Put_UpdatesExistingPlatform()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();
        var updatedPlatform = new UpdatePlatformDto
        {
            Name = "Super Nintendo Entertainment System",
        };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync("/api/platforms/1", updatedPlatform);
        Platform? platform = await response.Content.ReadFromJsonAsync<Platform>();

        // Assert
        response.EnsureSuccessStatusCode();
        platform.Should().NotBeNull();
        platform.PlatformId.Should().Be(1);
        platform.Name.Should().Be(updatedPlatform.Name);
    }

    [Fact]
    public async Task Delete_RemovesPlatform()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.DeleteAsync("/api/platforms/1");

        // Assert
        response.EnsureSuccessStatusCode();
        HttpResponseMessage getResponse = await client.GetAsync("/api/platforms/1");
        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistentPlatform_ReturnsNotFound()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.DeleteAsync("/api/platforms/999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
