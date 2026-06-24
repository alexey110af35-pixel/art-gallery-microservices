using Gallery.API.Data;
using Gallery.API.Extensions;
using Gallery.API.Kafka;
using Gallery.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaintingsController : ControllerBase
{
	private readonly ILogger<PaintingsController> _logger;
	private readonly AppDbContext _context;
	private readonly GalleryEventProducer _eventProducer;

	public PaintingsController(ILogger<PaintingsController> logger, AppDbContext context, GalleryEventProducer eventProducer)
	{
		_logger = logger;
		_context = context;
		_eventProducer = eventProducer;
	}

	// GET: api/paintings
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PaintingResponseDto>>> GetPaintings()
	{
		var paitings = await _context.Paintings
			.AsNoTracking()
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();

		return paitings.ToDtoList();
	}

	// GET: api/paintings/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<PaintingResponseDto>> GetPainting(Guid id)
	{
		var painting = await _context.Paintings
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id);

		if (painting == null)
			return NotFound();

		return painting.ToDto();
	}

	// POST: api/paintings
	[HttpPost]
	public async Task<ActionResult<PaintingResponseDto>> CreatePainting([FromBody] CreatePaintingDto dto)
	{
		var painting = dto.ToEntity();

		_context.Paintings.Add(painting);
		await _context.SaveChangesAsync();

		// Публикуем событие в Kafka для асинхронной загрузки файла
		var uploadRequest = new ImageUploadRequestedEvent
		{
			PaintingId = painting.Id,
			FileName = $"{painting.Title.Replace(" ", "_")}.jpg", // В реальности имя приходит от клиента
			ContentType = "image/jpeg",
			FileSize = 0 // В реальности размер передаётся
		};

		_logger.LogInformation($"Publishing image upload request for painting {painting.Id}");
		await _eventProducer.PublishImageUploadRequestedAsync(uploadRequest);
		_logger.LogInformation($"Image upload request published for painting {painting.Id}");

		return CreatedAtAction(nameof(GetPainting), new { id = painting.Id }, painting);
	}

	// PUT: api/paintings/{id}
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdatePainting(Guid id, [FromBody] UpdatePaintingDto dto)
	{
		var painting = await _context.Paintings.FindAsync(id);
		if (painting == null)
			return NotFound();

		painting.Title = dto.Title;
		painting.Artist = dto.Artist;
		painting.Year = dto.Year;
		painting.Description = dto.Description;
		painting.ImageUrl = dto.ImageUrl ?? painting.ImageUrl;
		painting.UpdatedAt = DateTime.UtcNow;

		await _context.SaveChangesAsync();
		return NoContent();
	}

	// DELETE: api/paintings/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePainting(Guid id)
	{
		var painting = await _context.Paintings.FindAsync(id);
		if (painting == null)
			return NotFound();

		_context.Paintings.Remove(painting);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}