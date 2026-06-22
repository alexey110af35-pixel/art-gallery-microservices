using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data;

public class AppDbContext : DbContext
{
	private readonly IConfiguration _configuration;

	public DbSet<User> Users { get; set; }

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
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.Username).IsUnique();
			entity.HasIndex(e => e.Email).IsUnique();
			entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
			entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
			entity.Property(e => e.PasswordHash).IsRequired();
			entity.Property(e => e.Role).HasMaxLength(20);
		});

		// Создаём администратора по умолчанию (для тестов)
		base.OnModelCreating(modelBuilder);
	}
}