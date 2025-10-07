namespace Games.DTOs;

public class UpdateGenreDto
{
    public string Name { get; set; } = string.Empty;
    public List<int> Games { get; set; } = new();
}