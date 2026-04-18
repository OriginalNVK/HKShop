using HKShop.Models;
using Microsoft.EntityFrameworkCore;
using HKShop.Repositories.Interfaces;

namespace HKShop.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
	private readonly DBContext _context;

	public InvoiceRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Invoices
			.AsNoTracking()
			.Include(i => i.CustomerIdNavigation)
			.Include(i => i.AdminIdNavigation)
			.OrderByDescending(i => i.OrderDate)
			.ToListAsync(cancellationToken);
	}

	public async Task<Invoice?> GetByIdAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		return await _context.Invoices
			.Include(i => i.CustomerIdNavigation)
			.Include(i => i.AdminIdNavigation)
			.Include(i => i.DetailInvoices)
				.ThenInclude(d => d.ProductIdNavigation)
			.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
	}

	public async Task<List<Invoice>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
	{
		return await _context.Invoices
			.AsNoTracking()
			.Where(i => i.CustomerId == customerId)
			.OrderByDescending(i => i.OrderDate)
			.ToListAsync(cancellationToken);
	}

	public async Task<Invoice> CreateAsync(Invoice invoice, CancellationToken cancellationToken = default)
	{
		await _context.Invoices.AddAsync(invoice, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return invoice;
	}

	public async Task<bool> UpdateStatusAsync(int invoiceId, int statusCode, DateOnly? deliveryDate = null, CancellationToken cancellationToken = default)
	{
		var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
		if (invoice == null)
		{
			return false;
		}

		invoice.StatusCode = statusCode;
		invoice.DeliveryDate = deliveryDate;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> AssignAdminAsync(int invoiceId, string adminId, CancellationToken cancellationToken = default)
	{
		var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
		if (invoice == null)
		{
			return false;
		}

		invoice.AdminId = adminId;
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
		if (invoice == null)
		{
			return false;
		}

		var details = await _context.DetailInvoices
			.Where(d => d.InvoiceId == invoiceId)
			.ToListAsync(cancellationToken);

		if (details.Count > 0)
		{
			_context.DetailInvoices.RemoveRange(details);
		}

		_context.Invoices.Remove(invoice);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
