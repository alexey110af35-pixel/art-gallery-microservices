using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public class JwtService
{
	private readonly IConfiguration _configuration;

	public JwtService(IConfiguration configuration)
	{
		_configuration = configuration;
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

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
			_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured")));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
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