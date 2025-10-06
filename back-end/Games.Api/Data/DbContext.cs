
using Microsoft.EntityFrameworkCore;
using Games.Models;

namespace Games.Data;


public class GamesDbContext : DbContext
{
    // Constructor
    public GamesDbContext(DbContextOptions<GamesDbContext> options) : base(options) { }

    // DbSets (tables)
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Platform> Platforms { get; set; } = null!;
    public DbSet<GamePlatform> GamePlatforms { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<GameGenre> GameGenres { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserGenre> UserGenres { get; set; } = null!;
    public DbSet<UserGame> UserGames { get; set; } = null!;



    //public DbSet<User> User { get; set; } = null!;
    //  Model configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for Game and Platform join table
        modelBuilder.Entity<GamePlatform>()
            .HasKey(gc => new { gc.GameId, gc.PlatformId });

        modelBuilder.Entity<GameGenre>()
            .HasKey(gc => new { gc.GameId, gc.GenreId });

        modelBuilder.Entity<UserGenre>()
            .HasKey(ug => new { ug.UserId, ug.GenreId });

        modelBuilder.Entity<UserGame>()
            .HasKey(ug => new { ug.UserId, ug.GameId });

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

        // Game to Genre
        modelBuilder.Entity<GameGenre>()
            .HasOne(gp => gp.Game)
            .WithMany(g => g.GameGenres)
            .HasForeignKey(gp => gp.GameId);

        // Genre to Game
        modelBuilder.Entity<GameGenre>()
            .HasOne(gp => gp.Genre)
            .WithMany(p => p.GameGenres)
            .HasForeignKey(gp => gp.GenreId);

        // Rating Composite Primary Key
        modelBuilder.Entity<Rating>()
            .HasKey(r => new { r.UserId, r.GameId });

        // Rating -> Game (many-to-one)
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Game)
            .WithMany(g => g.Ratings)
            .HasForeignKey(r => r.GameId);

        // Rating -> User (many-to-one)
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Rating>()
            .Property(r => r.Title)
            .IsRequired();

        modelBuilder.Entity<Rating>()
            .Property(r => r.Rate)
            .IsRequired();

        // User to Genre
        modelBuilder.Entity<UserGenre>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGenres)
            .HasForeignKey(ug => ug.UserId);

        // User to game
        modelBuilder.Entity<UserGame>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.GameLibrary)
            .HasForeignKey(ug => ug.UserId);

        // Genre to user
        // modelBuilder.Entity<UserGenre>()
        //     .HasOne(ug => ug.Genre)
        //     .WithMany(u => u.UserGenres)
        //     .HasForeignKey(ug => ug.GenreId);

    }
}