using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

public static class DbExtensions
{
	public static IServiceCollection AddPostgresDbContext<TContext>(
		this IServiceCollection services,
		string connectionString)
		where TContext : DbContext
	{
		services.AddDbContext<TContext>(options =>
			options.UseNpgsql(connectionString));

		return services;
	}
}