using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Games.Data;

namespace Games.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<GamesDbContext>>();
            services.RemoveAll<GamesDbContext>();
            services.AddDbContext<GamesDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "testDb")
            );
        });

        builder.UseEnvironment("Testing");
    }

    public async Task SeedDatabaseAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        GamesDbContext dbContext = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
        dbContext.Database.EnsureCreated();
        await Utilities.SeedTestDbAsync(dbContext);
    }
}
