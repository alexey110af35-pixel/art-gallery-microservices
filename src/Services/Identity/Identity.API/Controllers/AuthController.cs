using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly JwtService _jwtService;

	public AuthController(AppDbContext context, JwtService jwtService)
	{
		_context = context;
		_jwtService = jwtService;
	}

	// POST: api/auth/register
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
	{
		// Проверяем, существует ли пользователь
		var existingUser = await _context.Users
			.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);

		if (existingUser != null)
		{
			return BadRequest("User with this username or email already exists");
		}

		// Создаём пользователя
		var user = new User
		{
			Id = Guid.NewGuid(),
			Username = dto.Username,
			Email = dto.Email,
			PasswordHash = HashPassword(dto.Password),
			Role = "User", // по умолчанию
			CreatedAt = DateTime.UtcNow
		};

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		// Генерируем токен
		var token = _jwtService.GenerateToken(user);

		return Ok(new AuthResponseDto
		{
			Token = token,
			Username = user.Username,
			Role = user.Role,
			ExpiresAt = _jwtService.GetExpirationDate()
		});
	}

	// POST: api/auth/login
	[HttpPost("login")]
	public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
	{
		var user = await _context.Users
			.FirstOrDefaultAsync(u => u.Username == dto.Username);

		if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
		{
			return Unauthorized("Invalid username or password");
		}

		// Обновляем время последнего входа
		user.LastLoginAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		var token = _jwtService.GenerateToken(user);

		return Ok(new AuthResponseDto
		{
			Token = token,
			Username = user.Username,
			Role = user.Role,
			ExpiresAt = _jwtService.GetExpirationDate()
		});
	}

	// GET: api/auth/me (проверка токена)
	[Authorize]
	[HttpGet("me")]
	public async Task<ActionResult<User>> GetCurrentUser()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId == null)
		{
			return Unauthorized();
		}

		var user = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

		if (user == null)
		{
			return NotFound();
		}

		// Не возвращаем хэш пароля
		user.PasswordHash = "";
		return user;
	}

	// Вспомогательные методы
	private static string HashPassword(string password)
	{
		using var sha256 = SHA256.Create();
		var bytes = Encoding.UTF8.GetBytes(password);
		var hash = sha256.ComputeHash(bytes);
		return Convert.ToBase64String(hash);
	}

	private static bool VerifyPassword(string password, string hash)
	{
		return HashPassword(password) == hash;
	}
}