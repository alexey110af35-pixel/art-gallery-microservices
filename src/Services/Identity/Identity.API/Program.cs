using Identity.API.Data;
using Identity.API.Domain.Repositories;
using Identity.API.Infrastructure.Data;
using Identity.API.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();
builder.Services.AddJwtAuthentication();
builder.Services.AddCustomSwagger("Identity API");

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddControllers();

var app = builder.Build();

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