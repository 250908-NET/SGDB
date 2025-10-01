public class RatingDto
{
    public int GameID { get; set; }
    public int UserID { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int Rate { get; set; }
    public DateTime DateTimeRated { get; set; }
}
