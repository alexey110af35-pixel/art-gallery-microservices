using Shared.Extensions;
using Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();
builder.Services.AddJwtAuthentication();
//builder.Services.AddCustomSwagger("Art Gallery API Gateway");

// ===== —œ≈÷»‘»◊Õ€≈ ƒÀﬂ GATEWAY =====
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseGlobalExceptionHandler();

//if (app.Environment.IsDevelopment())
//{
//	app.UseSwagger();
//	app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();