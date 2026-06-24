using Gallery.API.Data;
using Gallery.API.Kafka;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();
builder.Services.AddJwtAuthentication();
builder.Services.AddSingleton<GalleryEventProducer>();
builder.Services.AddHostedService<GalleryEventConsumer>();
builder.Services.AddCustomSwagger("Gallery API");

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();

var app = builder.Build();

Console.WriteLine("Gallery.API started, waiting for requests...");

app.UseGlobalExceptionHandler();

// Миграции
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();