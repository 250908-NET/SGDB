using Microsoft.EntityFrameworkCore;

namespace Games.Models;

public class User
{
    public int UserID { get; set; }
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}