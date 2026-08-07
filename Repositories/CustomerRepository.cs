using HKShop.Domain;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class CustomerRepository : ICustomerRepository
{
	private readonly HKShopDbContext _context;

	public CustomerRepository(HKShopDbContext context)
	{
		_context = context;
	}

	public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.AsNoTracking()
			.Include(c => c.User)
			.OrderBy(c => c.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.User.Username == username, cancellationToken);
	}

		public async Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.Include(c => c.User)
			.Include(c => c.Carts)
			.Include(c => c.Invoices)
				.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public async Task<Customer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
	}

	public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
	{
		await _context.Customers.AddAsync(customer, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return customer;
	}

	public async Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.Fullname = customer.Fullname;
		existing.Gender = customer.Gender;
		existing.Birthday = customer.Birthday;
		existing.Address = customer.Address;
		existing.Phone = customer.Phone;
		existing.Email = customer.Email;
		existing.Avatar = customer.Avatar;
		existing.UserId = customer.UserId;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int customerId, CancellationToken cancellationToken = default)
	{
		var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
		if (customer == null)
		{
			return false;
		}

		_context.Customers.Remove(customer);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
