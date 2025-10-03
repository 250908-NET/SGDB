using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    public string username { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public ICollection<Game> GameLibrary { get; set; } = new List<Game>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public static implicit operator List<object>(User v)
    {
        throw new NotImplementedException();
    }
}