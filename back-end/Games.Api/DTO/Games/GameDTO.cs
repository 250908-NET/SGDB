namespace Games.DTOs;

public class GameDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    
    public List<string> Platforms { get; set; } = new();
}