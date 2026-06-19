namespace Gallery.API.Models;

public class Painting
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Artist { get; set; } = string.Empty;
	public int Year { get; set; }
	public string Description { get; set; } = string.Empty;
	public string ImageUrl { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}