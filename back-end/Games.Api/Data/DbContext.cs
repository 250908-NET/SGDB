
using Microsoft.EntityFrameworkCore;
using Games.Models;

namespace Games.Data;


public class GamesDbContext  : DbContext
{
    // Constructor
    public GamesDbContext (DbContextOptions<GamesDbContext > options) : base(options) {}

    // DbSets (tables)
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Platform> Platforms { get; set; } = null!;
    public DbSet<GamePlatform> GamePlatforms { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<GameGenre> GameGenres { get; set; } = null!;

    //  Model configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for Game and Platform join table
        modelBuilder.Entity<GamePlatform>()
            .HasKey(gc => new { gc.GameId, gc.PlatformId });

        modelBuilder.Entity<GameGenre>()
            .HasKey(gc => new { gc.GameId, gc.GenreId });

        // Relationships

        // Game to Platform
        modelBuilder.Entity<GamePlatform>()
            .HasOne(gp => gp.Game)
            .WithMany(g => g.GamePlatforms)
            .HasForeignKey(gp => gp.GameId);

        // Platform to Game
        modelBuilder.Entity<GamePlatform>()
            .HasOne(gp => gp.Platform)
            .WithMany(p => p.GamePlatforms)
            .HasForeignKey(gp => gp.PlatformId);

        // Game to Publisher
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Publisher)
            .WithMany(c => c.PublishedGames)
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Game to Developer
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Developer)
            .WithMany(c => c.DevelopedGames)
            .HasForeignKey(g => g.DeveloperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
// adding this to the file to make a change

        
        // Game to Genre
        // modelBuilder.Entity<GameGenre>()
        //     .HasOne(gp => gp.Game)
        //     .WithMany(g => g.GameGenres)
        //     .HasForeignKey(gp => gp.GameId);

        // // Genre to Game
        // modelBuilder.Entity<GameGenre>()
        //     .HasOne(gp => gp.Genre)
        //     .WithMany(p => p.GameGenres)
        //     .HasForeignKey(gp => gp.GenreId);