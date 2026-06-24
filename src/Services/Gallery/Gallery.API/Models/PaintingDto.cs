namespace Gallery.API.Models;

public class CreatePaintingDto
{
	public string Title { get; set; } = string.Empty;
	public string Artist { get; set; } = string.Empty;
	public int Year { get; set; }
	public string Description { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
}

public class UpdatePaintingDto
{
	public string Title { get; set; } = string.Empty;
	public string Artist { get; set; } = string.Empty;
	public int Year { get; set; }
	public string Description { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
}

public class PaintingResponseDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Artist { get; set; } = string.Empty;
	public int Year { get; set; }
	public string Description { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
	public PaintingStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}