using System.ComponentModel.DataAnnotations;

public class RatingDto
{
    [Required]
    public int GameId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
    public int Rate { get; set; }

    public DateTime DateTimeRated { get; set; }
}
