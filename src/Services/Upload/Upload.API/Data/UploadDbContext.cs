using Microsoft.EntityFrameworkCore;
using Upload.API.Models;

namespace Upload.API.Data;

public class UploadDbContext : DbContext
{
	private readonly IConfiguration _configuration;

	public DbSet<ImageModel> Images { get; set; }

	public UploadDbContext(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
			.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
	}
}