using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Games.Models;

public class Genre
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GenreId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;   // ex. "PlayStation 5"

    // Many to many relationship
    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}