using HKShop.Models;
using Microsoft.EntityFrameworkCore;
using HKShop.Repositories.Interfaces;

namespace HKShop.Repositories;

public class UserRepository : IUserRepository
{
	private readonly DBContext _context;

	public UserRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.AsNoTracking()
			.OrderByDescending(u => u.CreatedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Customer)
			.Include(u => u.Admin)
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
	}

	public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
	}

	public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.AsNoTracking()
			.AnyAsync(u => u.Username == username, cancellationToken);
	}

	public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
	{
		await _context.Users.AddAsync(user, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return user;
	}

	public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
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
		var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.Users.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
