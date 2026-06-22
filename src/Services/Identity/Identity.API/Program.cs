using Identity.API.Data;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Подключение к PostgreSQL
builder.Services.AddDbContext<AppDbContext>();

// Добавляем JWT-сервис
builder.Services.AddScoped<JwtService>();

// Читаем JWT-настройки из переменных окружения
var secretKey = Environment.GetEnvironmentVariable("JWT__SECRET_KEY")
	?? throw new InvalidOperationException("JWT__SECRET_KEY not set");
var issuer = Environment.GetEnvironmentVariable("JWT__ISSUER") ?? "ArtGallery";
var audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE") ?? "ArtGalleryUsers";

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
	});


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Автоматическое применение миграций
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