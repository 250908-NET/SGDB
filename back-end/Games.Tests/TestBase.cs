using Microsoft.EntityFrameworkCore;
using Games.Data;

namespace Games.Tests;

public class TestBase : IDisposable
{
    protected GamesDbContext Context { get; private set; }

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<GamesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        Context = new GamesDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    protected async Task SeedDataAsync()
    {
        await Utilities.SeedTestDbAsync(Context);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
