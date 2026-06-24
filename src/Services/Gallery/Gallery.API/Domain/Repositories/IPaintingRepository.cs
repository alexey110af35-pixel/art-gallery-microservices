using Gallery.API.Models;

namespace Gallery.API.Domain.Repositories;

public interface IPaintingRepository
{
	Task<Painting?> GetByIdAsync(Guid id);
	Task<IEnumerable<Painting>> GetAllAsync();
	Task AddAsync(Painting painting);
	void Update(Painting painting);
	void Delete(Painting painting);
	Task SaveChangesAsync();
}