using Gallery.API.Domain.Repositories;
using Gallery.API.Extensions;
using Gallery.API.Kafka;
using Gallery.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaintingsController : ControllerBase
{
	private readonly IPaintingRepository _repository;
	private readonly GalleryEventProducer _eventProducer;
	private readonly ILogger<PaintingsController> _logger;

	public PaintingsController(
		IPaintingRepository repository,
		GalleryEventProducer eventProducer,
		ILogger<PaintingsController> logger
		)
	{
		_logger = logger;
		_eventProducer = eventProducer;
		_repository = repository;
	}

	// GET: api/paintings
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PaintingResponseDto>>> GetPaintings()
	{
		var paitings = await _repository.GetAllAsync();
		return paitings.ToDtoList();
	}

	// GET: api/paintings/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<PaintingResponseDto>> GetPainting(Guid id)
	{
		var painting = await _repository.GetByIdAsync(id);

		if (painting == null)
			return NotFound();

		return painting.ToDto();
	}

	// POST: api/paintings
	[HttpPost]
	public async Task<ActionResult<PaintingResponseDto>> CreatePainting([FromBody] CreatePaintingDto dto)
	{
		var painting = dto.ToEntity();

		await _repository.AddAsync(painting);
		await _repository.SaveChangesAsync();

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
		var painting = await _repository.GetByIdAsync(id);
		if (painting == null)
			return NotFound();

		dto.ToEntity(painting);

		_repository.Update(painting);
		await _repository.SaveChangesAsync();

		return NoContent();
	}

	// DELETE: api/paintings/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePainting(Guid id)
	{
		var painting = await _repository.GetByIdAsync(id);
		if (painting == null)
			return NotFound();

		_repository.Delete(painting);
		await _repository.SaveChangesAsync();

		return NoContent();
	}
}