using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Games.Models;

public class Company
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CompanyId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Game> DevelopedGames { get; set; } = new List<Game>();
    public ICollection<Game> PublishedGames { get; set; } = new List<Game>();
}