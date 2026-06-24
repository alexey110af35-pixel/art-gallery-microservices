using Identity.API.Domain.Repositories;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IUserRepository _repository;
	private readonly JwtService _jwtService;

	public AuthController(IUserRepository repository, JwtService jwtService)
	{
		_repository = repository;
		_jwtService = jwtService;
	}

	// POST: api/auth/register
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
	{
		// Проверяем, существует ли пользователь
		if (!await _repository.ExistsByEmailAsync(dto.Email))
			return BadRequest("User with this username already exists");

		if (await _repository.ExistsByEmailAsync(dto.Email))
			return BadRequest("User with this email already exists");

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

		await _repository.AddAsync(user);
		await _repository.SaveChangesAsync();

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
		var user = await _repository.GetByUsernameAsync(dto.Username);
		if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
		{
			return Unauthorized("Invalid username or password");
		}

		// Обновляем время последнего входа
		user.LastLoginAt = DateTime.UtcNow;

		_repository.Update(user);
		await _repository.SaveChangesAsync();

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
		if (userId == null || !Guid.TryParse(userId, out var id))
		{
			return Unauthorized();
		}

		var user = await _repository.GetByIdAsync(id);
		if (user == null)
		{
			return NotFound();
		}

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