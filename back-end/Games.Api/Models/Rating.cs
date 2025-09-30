using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Games.Models;

public class Rating
{
    [ForeignKey("Game")]
    public int GameID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    [Range(1, 10)]
    public int Rate { get; set; }             //Possible renaming

    public DateTime DateTimeRated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Game Game { get; set; }
    public User User { get; set; }
}