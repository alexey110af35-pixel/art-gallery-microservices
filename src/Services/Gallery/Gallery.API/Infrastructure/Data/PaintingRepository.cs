using Gallery.API.Data;
using Gallery.API.Domain.Repositories;
using Gallery.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Infrastructure.Data;

public class PaintingRepository : IPaintingRepository
{
	private readonly AppDbContext _context;

	public PaintingRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Painting?> GetByIdAsync(Guid id)
	{
		return await _context.Paintings
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id);
	}

	public async Task<IEnumerable<Painting>> GetAllAsync()
	{
		return await _context.Paintings
			.AsNoTracking()
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();
	}

	public async Task AddAsync(Painting painting)
	{
		await _context.Paintings.AddAsync(painting);
	}

	public void Update(Painting painting)
	{
		_context.Paintings.Update(painting);
	}

	public void Delete(Painting painting)
	{
		_context.Paintings.Remove(painting);
	}

	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}
}