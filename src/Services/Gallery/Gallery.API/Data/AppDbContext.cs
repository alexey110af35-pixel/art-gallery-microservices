using Gallery.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Painting> Paintings { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Painting>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Artist).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.Property(e => e.ImageUrl).HasMaxLength(500);
			entity.HasIndex(e => e.Title);
		});

		base.OnModelCreating(modelBuilder);
	}
}