using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Games.Models;

// Joined table
public class UserGenre
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}