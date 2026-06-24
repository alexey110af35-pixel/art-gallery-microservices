namespace Upload.API.Models;

public class ImageModel
{
	public Guid Id { get; set; }
	public string FileName { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
	public long Size { get; set; }
	public string StoragePath { get; set; } = string.Empty;
	public DateTime UploadedAt { get; set; }
}