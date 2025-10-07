namespace Games.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string username { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;

    public List<int> UserGenres { get; set; } = new List<int>();
    public List<int> GameLibrary { get; set; } = new List<int>();
}