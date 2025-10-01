using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Games.Models;

public class Rating
{
    public int GameID { get; set; }
    public int UserID { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    [Range(1, 10)]
    public int Rate { get; set; }             //Possible renaming

    public DateTime DateTimeRated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("GameID")]
    public Game Game { get; set; } = null!;
    [ForeignKey("UserID")]
    public User User { get; set; } = null!;
}