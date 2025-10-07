namespace Games.DTOs;

public class CompanyDto
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<int> DevelopedGames { get; set; } = new();
    public List<int> PublishedGames { get; set; } = new();
}
