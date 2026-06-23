using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Config;
using System.Text;

namespace Shared.Extensions;

public static class JwtExtensions
{
	public static IServiceCollection AddJwtAuthentication(
		this IServiceCollection services)
	{
		// Читаем настройки из статического класса
		var secretKey = JwtSettings.SecretKey;
		var issuer = JwtSettings.Issuer;
		var audience = JwtSettings.Audience;

		// Проверяем длину ключа (для HS256 нужно >= 32 символов)
		if (secretKey.Length < 32)
		{
			throw new InvalidOperationException(
				$"JWT SecretKey is too short. Current length: {secretKey.Length} chars. " +
				"Minimum required: 32 chars (256 bits) for HS256 algorithm.");
		}

		var key = Encoding.UTF8.GetBytes(secretKey);

		// Настраиваем аутентификацию
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