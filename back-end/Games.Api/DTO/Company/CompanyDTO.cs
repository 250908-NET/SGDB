namespace Games.DTOs;

public class CompanyDto
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<string> DevelopedGames { get; set; } = new();
    public List<string> PublishedGames { get; set; } = new();
}
