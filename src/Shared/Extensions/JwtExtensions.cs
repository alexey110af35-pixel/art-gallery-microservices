using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared.Extensions;

public static class JwtExtensions
{
	public static IServiceCollection AddJwtAuthentication(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// 1. Пытаемся прочитать ключ из переменных окружения (.env)
		var secretKey = Environment.GetEnvironmentVariable("JWT__SECRET_KEY")
			?? configuration["Jwt:SecretKey"]
			?? throw new InvalidOperationException("JWT__SECRET_KEY is not set in .env or appsettings.json");

		// 2. Читаем остальные параметры с fallback на значения по умолчанию
		var issuer = Environment.GetEnvironmentVariable("JWT__ISSUER")
			?? configuration["Jwt:Issuer"]
			?? "ArtGallery";

		var audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE")
			?? configuration["Jwt:Audience"]
			?? "ArtGalleryUsers";

		// 3. Проверяем длину ключа (для HS256 нужно >= 32 символов)
		if (secretKey.Length < 32)
		{
			throw new InvalidOperationException(
				$"JWT SecretKey is too short. Current length: {secretKey.Length} chars. " +
				"Minimum required: 32 chars (256 bits) for HS256 algorithm.");
		}

		Console.WriteLine($"secretKey: {secretKey}");

		var key = Encoding.UTF8.GetBytes(secretKey);

		// 4. Настраиваем аутентификацию
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = issuer,
					ValidAudience = audience,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};

				// Опционально: логирование ошибок для отладки
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
						return Task.CompletedTask;
					}
				};
			});

		services.AddAuthorization();

		return services;
	}
}