namespace Games.DTOs;

public class PlatformDto
{
    public int PlatformId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Games { get; set; } = new();
}