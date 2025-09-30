using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Games.Models;

public class Platform
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PlatformId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;   // ex. "PlayStation 5"

    // Many to many relationship
    public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
}