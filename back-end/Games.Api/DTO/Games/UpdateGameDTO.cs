namespace Games.DTOs;

public class UpdateGameDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }

    public int DeveloperId { get; set; }   // FK to Company
    public int PublisherId { get; set; }   // FK to Company
}
