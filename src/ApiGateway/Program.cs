using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

string envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
DotNetEnv.Env.Load(envPath);
var builder = WebApplication.CreateBuilder(args);

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

// Добавляем YARP
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Важно: порядок Middleware
app.UseAuthentication(); // Сначала проверяем токен
app.UseAuthorization();  // Потом проверяем права
app.MapReverseProxy();   // Затем проксируем

app.Run();