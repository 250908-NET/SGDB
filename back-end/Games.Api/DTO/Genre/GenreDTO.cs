namespace Games.DTOs;

public class GenreDto
{
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Games { get; set; } = new();
}