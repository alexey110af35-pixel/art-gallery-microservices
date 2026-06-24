using Gallery.API.Models;

namespace Gallery.API.Extensions;

public static class MappingExtensions
{
	// Преобразование сущности в DTO
	public static PaintingResponseDto ToDto(this Painting painting)
	{
		return new PaintingResponseDto
		{
			Id = painting.Id,
			Title = painting.Title,
			Artist = painting.Artist,
			Year = painting.Year,
			Description = painting.Description,
			ImageUrl = painting.ImageUrl,
			Status = painting.Status,
			CreatedAt = painting.CreatedAt,
			UpdatedAt = painting.UpdatedAt
		};
	}

	// Преобразование списка сущностей в список DTO
	public static List<PaintingResponseDto> ToDtoList(this IEnumerable<Painting> paintings)
	{
		return paintings.Select(p => p.ToDto()).ToList();
	}


	public static Painting ToEntity(this CreatePaintingDto dto)
	{
		return new Painting
		{
			Id = Guid.NewGuid(),
			Title = dto.Title,
			Artist = dto.Artist,
			Year = dto.Year,
			Description = dto.Description,
			ImageUrl = dto.ImageUrl,
			Status = PaintingStatus.Pending,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
	}
}