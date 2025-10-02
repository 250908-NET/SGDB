namespace Games.Tests;

public class PlatformControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public PlatformControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/platforms")]
    public async Task Get_ReturnsSuccessAndCorrectContentType(string url)
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        await _factory.SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await client.GetAsync(url);
        string contentType = response.Content.Headers.ContentType?.ToString() ?? "";

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
}
