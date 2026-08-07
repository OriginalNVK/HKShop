using HKShop.Domain;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class EmployeeRepository: IEmployeeRepository
{
	private readonly HKShopDbContext _context;

	public EmployeeRepository(HKShopDbContext context)
	{
		_context = context;
	}

	public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Employees
			.AsNoTracking()
			.Include(e => e.User)
			.ToListAsync(cancellationToken);
	}

	public async Task<Employee?> GetByIdAsync(int employeeId, CancellationToken cancellationToken = default)
	{
		return await _context.Employees
			.Include(e => e.User)
			.Include(e => e.Invoices)
			.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
	}

	public async Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.Employees
			.Include(e => e.User)
			.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
	}

	public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
	{
		await _context.Employees.AddAsync(employee, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return employee;
	}

	public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.Fullname = employee.Fullname;
		existing.Email = employee.Email;
		existing.Phone = employee.Phone;
		existing.UserId = employee.UserId;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int employeeId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.Employees.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
