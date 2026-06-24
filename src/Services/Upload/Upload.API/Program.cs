using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Middleware;
using Upload.API.Data;
using Upload.API.Kafka;
using Upload.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();
builder.Services.AddJwtAuthentication();

// Подключение к PostgreSQL
builder.Services.AddDbContext<UploadDbContext>();

// MinIO
builder.Services.AddScoped<MinioService>();

// Kafka
builder.Services.AddSingleton<UploadEventProducer>();
builder.Services.AddHostedService<ImageRequestConsumer>();

// Controllers
builder.Services.AddControllers();

var bootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
Console.WriteLine($"Kafka BootstrapServers from config: '{bootstrapServers}'");

var app = builder.Build();

// Применяем миграции для Upload.API
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<UploadDbContext>();
	await dbContext.Database.MigrateAsync();
}

// Создаём бакет в MinIO
using (var scope = app.Services.CreateScope())
{
	var minioService = scope.ServiceProvider.GetRequiredService<MinioService>();
	await minioService.EnsureBucketExistsAsync();
}

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();