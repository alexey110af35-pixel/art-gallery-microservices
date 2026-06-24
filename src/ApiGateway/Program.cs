using Shared.Extensions;
using Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();
builder.Services.AddJwtAuthentication();

builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();