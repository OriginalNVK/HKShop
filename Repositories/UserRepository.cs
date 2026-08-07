using HKShop.Domain;
using Microsoft.EntityFrameworkCore;
using HKShop.Repositories.Interfaces;

namespace HKShop.Repositories;

public class UserRepository : IUserRepository
{
	private readonly HKShopDbContext _context;

	public UserRepository(HKShopDbContext context)
	{
		_context = context;
	}

	public async Task<List<AppUser>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.AppUsers
			.AsNoTracking()
			.OrderByDescending(u => u.CreatedDate)
			.ToListAsync(cancellationToken);
	}

	public async Task<AppUser?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.AppUsers
			.Include(u => u.Customer)
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
	}

	public async Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.AppUsers
			.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
	}

	public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.AppUsers
			.AsNoTracking()
			.AnyAsync(u => u.Username == username, cancellationToken);
	}

	public async Task<AppUser> CreateAsync(AppUser user, CancellationToken cancellationToken = default)
	{
		await _context.AppUsers.AddAsync(user, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return user;
	}

	public async Task<bool> UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
	{
		var existing = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.Username = user.Username;
		existing.Password = user.Password;
		existing.Role = user.Role;
		existing.IsActive = user.IsActive;
		existing.RandomKey = user.RandomKey;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int userId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.AppUsers.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
