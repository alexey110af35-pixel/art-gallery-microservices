using Identity.API.Data;
using Identity.API.Domain.Repositories;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Infrastructure.Data;

public class UserRepository : IUserRepository
{
	private readonly AppDbContext _context;

	public UserRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<User?> GetByUsernameAsync(string username)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Username == username);
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<User?> GetByIdAsync(Guid id)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task<bool> ExistsByUsernameAsync(string username)
	{
		return await _context.Users.AnyAsync(u => u.Username == username);
	}

	public async Task<bool> ExistsByEmailAsync(string email)
	{
		return await _context.Users.AnyAsync(u => u.Email == email);
	}

	public async Task AddAsync(User user)
	{
		await _context.Users.AddAsync(user);
	}

	public void Update(User user)
	{
		_context.Users.Update(user);
	}

	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}
}