namespace Gallery.API.Models;

public enum PaintingStatus
{
	Pending = 0,
	Processing = 1,
	Ready = 2,
	Failed = 3
}

public class Painting
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Artist { get; set; } = string.Empty;
	public int Year { get; set; }
	public string Description { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
	public PaintingStatus Status { get; set; } = PaintingStatus.Pending;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}