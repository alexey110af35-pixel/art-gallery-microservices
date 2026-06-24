using Gallery.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Data;

public class AppDbContext : DbContext
{
	private readonly IConfiguration _configuration;

	public DbSet<Painting> Paintings { get; set; }

	public AppDbContext(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
			.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Painting>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Artist).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.Property(e => e.ImageUrl).HasMaxLength(500);
			entity.Property(e => e.Status)
			.HasConversion<string>()
			.HasMaxLength(20)
			.HasDefaultValue(PaintingStatus.Pending);

			entity.HasIndex(e => e.Title);
		});

		base.OnModelCreating(modelBuilder);
	}
}