using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GameId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [MaxLength(100)]
    public Company Developer { get; set; } = null!;
    public Company Publisher { get; set; } = null!;

    // Foreign keys
    public int DeveloperId { get; set; }
    public int PublisherId { get; set; }

    // Many to many relationship
    public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

    public ICollection<User> UsersList { get; set; } = new List<User>();
}