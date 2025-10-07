namespace Games.DTOs;

public class GenreDto
{
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> Games { get; set; } = new();
}