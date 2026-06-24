using Identity.API.Models;

namespace Identity.API.Domain.Repositories;

public interface IUserRepository
{
	Task<User?> GetByUsernameAsync(string username);
	Task<User?> GetByEmailAsync(string email);
	Task<User?> GetByIdAsync(Guid id);
	Task<bool> ExistsByUsernameAsync(string username);
	Task<bool> ExistsByEmailAsync(string email);
	Task AddAsync(User user);
	void Update(User user);
	Task SaveChangesAsync();
}