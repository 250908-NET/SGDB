namespace Games.DTOs;

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;

    public List<int> DevelopedGames { get; set; } = new();
    public List<int> PublishedGames { get; set; } = new();
}
