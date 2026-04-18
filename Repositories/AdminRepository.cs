using HKShop.Models;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class AdminRepository: IAdminRepository
{
	private readonly DBContext _context;

	public AdminRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Admin>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Admin
			.AsNoTracking()
			.Include(a => a.User)
			.ToListAsync(cancellationToken);
	}

	public async Task<Admin?> GetByIdAsync(string adminId, CancellationToken cancellationToken = default)
	{
		return await _context.Admin
			.Include(a => a.User)
			.Include(a => a.Invoices)
			.FirstOrDefaultAsync(a => a.AdminId == adminId, cancellationToken);
	}

	public async Task<Admin?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.Admin
			.Include(a => a.User)
			.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
	}

	public async Task<Admin> CreateAsync(Admin admin, CancellationToken cancellationToken = default)
	{
		await _context.Admin.AddAsync(admin, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return admin;
	}

	public async Task<bool> UpdateAsync(Admin admin, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Admin.FirstOrDefaultAsync(a => a.AdminId == admin.AdminId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.FullName = admin.FullName;
		existing.Email = admin.Email;
		existing.PhoneNumber = admin.PhoneNumber;
		existing.UserId = admin.UserId;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(string adminId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Admin.FirstOrDefaultAsync(a => a.AdminId == adminId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.Admin.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
