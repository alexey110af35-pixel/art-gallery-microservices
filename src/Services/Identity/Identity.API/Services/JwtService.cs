using Identity.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Services
{
	public class JwtService
	{
		private readonly IConfiguration _configuration;
		private readonly string _secretKey;
		private readonly string _issuer;
		private readonly string _audience;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;

			// Читаем ключ из .env или appsettings.json (как в JwtExtensions)
			_secretKey = Environment.GetEnvironmentVariable("JWT__SECRET_KEY")
				?? _configuration["Jwt:SecretKey"]
				?? throw new InvalidOperationException("JWT SecretKey not configured");

			_issuer = Environment.GetEnvironmentVariable("JWT__ISSUER")
				?? _configuration["Jwt:Issuer"]
				?? "ArtGallery";

			_audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE")
				?? _configuration["Jwt:Audience"]
				?? "ArtGalleryUsers";
		}

		public string GenerateToken(User user)
		{
			var claims = new[]
			{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role)
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public DateTime GetExpirationDate()
		{
			return DateTime.UtcNow.AddHours(1);
		}
	}
}