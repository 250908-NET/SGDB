using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Games.Models;

// Joined table
public class GameGenre
{
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}