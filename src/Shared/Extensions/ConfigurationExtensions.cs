using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace Shared.Extensions;

public static class ConfigurationExtensions
{
	public static IConfigurationBuilder LoadDotEnv(this IConfigurationBuilder builder)
	{
		// Ищем .env, начиная с текущей директории и поднимаясь вверх
		var envPath = FindDotEnvPath();

		if (!string.IsNullOrEmpty(envPath))
		{
			Env.Load(envPath);
			Console.WriteLine($"[Shared] .env loaded from: {envPath}");
		}
		else
		{
			Console.WriteLine("[Shared] .env file not found. Using system environment variables.");
		}

		builder.AddEnvironmentVariables();
		return builder;
	}

	private static string? FindDotEnvPath()
	{
		// Начинаем поиск с текущей директории (где лежит исполняемый файл)
		var directory = AppDomain.CurrentDomain.BaseDirectory;

		// Или используем Directory.GetCurrentDirectory() для запуска из папки проекта
		// var directory = Directory.GetCurrentDirectory();

		while (true)
		{
			var envPath = Path.Combine(directory, ".env");
			if (File.Exists(envPath))
			{
				return envPath;
			}

			// Поднимаемся на уровень выше
			var parentDirectory = Directory.GetParent(directory);
			if (parentDirectory == null)
			{
				// Дошли до корня диска — файл не найден
				return null;
			}

			directory = parentDirectory.FullName;
		}
	}
}