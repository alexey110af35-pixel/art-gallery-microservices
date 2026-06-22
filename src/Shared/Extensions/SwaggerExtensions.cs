using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Shared.Extensions;

public static class SwaggerExtensions
{
	public static IServiceCollection AddCustomSwagger(
		this IServiceCollection services,
		string title,
		string version = "v1")
	{
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Введите токен в формате: `Bearer {ваш_токен}`"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});

		return services;
	}
}