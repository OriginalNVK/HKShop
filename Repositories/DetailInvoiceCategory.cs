using HKShop.Models;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class DetailInvoiceRepository : IDetailInvoiceRepository
{
	private readonly DBContext _context;

	public DetailInvoiceRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<DetailInvoice>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailInvoices
			.AsNoTracking()
			.Include(d => d.ProductIdNavigation)
			.Where(d => d.InvoiceId == invoiceId)
			.ToListAsync(cancellationToken);
	}

	public async Task<DetailInvoice?> GetByIdAsync(int detailInvoiceId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailInvoices
			.Include(d => d.ProductIdNavigation)
			.Include(d => d.InvoiceIdNavigation)
			.FirstOrDefaultAsync(d => d.DetailInvoiceId == detailInvoiceId, cancellationToken);
	}

	public async Task<DetailInvoice> CreateAsync(DetailInvoice detail, CancellationToken cancellationToken = default)
	{
		await _context.DetailInvoices.AddAsync(detail, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return detail;
	}

	public async Task<bool> UpdateAsync(DetailInvoice detail, CancellationToken cancellationToken = default)
	{
		var existing = await _context.DetailInvoices.FirstOrDefaultAsync(d => d.DetailInvoiceId == detail.DetailInvoiceId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.InvoiceId = detail.InvoiceId;
		existing.ProductId = detail.ProductId;
		existing.Amount = detail.Amount;
		existing.Quantity = detail.Quantity;
		existing.Discount = detail.Discount;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int detailInvoiceId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.DetailInvoices.FirstOrDefaultAsync(d => d.DetailInvoiceId == detailInvoiceId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.DetailInvoices.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> ExistsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailInvoices
			.AsNoTracking()
			.AnyAsync(d => d.ProductId == productId, cancellationToken);
	}

	public async Task<decimal> GetInvoiceSubTotalAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailInvoices
			.AsNoTracking()
			.Where(d => d.InvoiceId == invoiceId)
			.SumAsync(d => d.Amount * d.Quantity * (1m - d.Discount / 100m), cancellationToken);
	}
}
