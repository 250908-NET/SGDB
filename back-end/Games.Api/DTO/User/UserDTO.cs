namespace Games.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string username { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;

    public List<string> UserGenres { get; set; } = new List<string>();
    public List<string> GameLibrary { get; set; } = new List<string>();
}