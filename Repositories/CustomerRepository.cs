using HKShop.Models;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class CustomerRepository : ICustomerRepository
{
	private readonly DBContext _context;

	public CustomerRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.AsNoTracking()
			.Include(c => c.User)
			.OrderBy(c => c.CustomerId)
			.ToListAsync(cancellationToken);
	}

	public async Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Customers
			.Include(c => c.User)
			.Include(c => c.Carts)
			.Include(c => c.Invoices)
			.FirstOrDefaultAsync(c => c.CustomerId == username, cancellationToken);
	}

	public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
	{
		await _context.Customers.AddAsync(customer, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return customer;
	}

	public async Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.FullName = customer.FullName;
		existing.Sex = customer.Sex;
		existing.BirthDate = customer.BirthDate;
		existing.Address = customer.Address;
		existing.PhoneNumber = customer.PhoneNumber;
		existing.Email = customer.Email;
		existing.Image = customer.Image;
		existing.UserId = customer.UserId;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(string customerId, CancellationToken cancellationToken = default)
	{
		var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
		if (customer == null)
		{
			return false;
		}

		_context.Customers.Remove(customer);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
