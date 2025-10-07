namespace Games.DTOs;

public class CreateGenreDto
{
    public string Name { get; set; } = string.Empty;
    public List<int> Games { get; set; } = new();
}