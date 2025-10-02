namespace Games.DTOs;

public class GameDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int PublisherId { get; set; }   // FK to Company
    public int DeveloperId { get; set; }   // FK to Company
    
    public List<string> Platforms { get; set; } = new();
    public List<string> Genres { get; set; } = new();
}