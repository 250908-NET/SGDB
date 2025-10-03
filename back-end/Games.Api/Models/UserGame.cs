using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Games.Models;

// Joined table
public class UserGame
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
}