using Gallery.API.Data;
using Gallery.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaintingsController : ControllerBase
{
	private readonly AppDbContext _context;

	public PaintingsController(AppDbContext context)
	{
		_context = context;
	}

	// GET: api/paintings
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Painting>>> GetPaintings()
	{
		return await _context.Paintings
			.AsNoTracking()
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();
	}

	// GET: api/paintings/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<Painting>> GetPainting(Guid id)
	{
		var painting = await _context.Paintings
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id);

		if (painting == null)
			return NotFound();

		return painting;
	}

	// POST: api/paintings
	[HttpPost]
	public async Task<ActionResult<Painting>> CreatePainting([FromBody] CreatePaintingDto dto)
	{
		var painting = new Painting
		{
			Id = Guid.NewGuid(),
			Title = dto.Title,
			Artist = dto.Artist,
			Year = dto.Year,
			Description = dto.Description,
			ImageUrl = dto.ImageUrl ?? "placeholder.jpg",
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};

		_context.Paintings.Add(painting);
		await _context.SaveChangesAsync();

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