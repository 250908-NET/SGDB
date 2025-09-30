using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Games.Models;

// Joined table
public class GamePlatform
{
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;
}