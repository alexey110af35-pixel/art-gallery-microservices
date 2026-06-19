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