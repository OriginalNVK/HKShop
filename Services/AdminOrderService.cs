using HKShop.DTOs;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminOrderService : IAdminOrderService
{
	private readonly HKShopDbContext _db;

	public AdminOrderService(HKShopDbContext db)
	{
		_db = db;
	}

	public async Task<List<DetailInvoiceVM>> GetDetailAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		return await _db.DetailInvoices
			.AsNoTracking()
			.Include(d => d.Product)
			.Where(d => d.InvoiceId == invoiceId)
			.Select(d => new DetailInvoiceVM
			{
				DetailInvoiceId = d.Id,
				InvoiceId = d.InvoiceId,
				ProductId = d.ProductId,
				Price = d.SubPrice,
				Quantity = d.Quantity,
				Discount = 0,
				ProductName = d.Product.Name,
				ProductImage = d.Product.Image ?? string.Empty,
				TotalAmount = d.SubPrice
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		var invoice = await _db.Invoices.FirstOrDefaultAsync(h => h.Id == invoiceId, cancellationToken);
		if (invoice == null)
		{
			return ServiceResult.Fail("Order not found");
		}

		var details = await _db.DetailInvoices.Where(c => c.InvoiceId == invoiceId).ToListAsync(cancellationToken);
		_db.DetailInvoices.RemoveRange(details);
		_db.Invoices.Remove(invoice);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Delete order successfully");
	}

	public async Task<ServiceResult> ConfirmAsync(int invoiceId, DateTime deliveryDate, string adminId, CancellationToken cancellationToken = default)
	{
		if (deliveryDate < DateTime.Now.Date)
		{
			return ServiceResult.Fail("Delivery date cannot be in the past");
		}

		await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			var invoice = await _db.Invoices.SingleOrDefaultAsync(h => h.Id == invoiceId, cancellationToken);
			if (invoice == null)
			{
				return ServiceResult.Fail("Order not found");
			}

			invoice.ShipmentDate = DateOnly.FromDateTime(deliveryDate);
			invoice.StatusId = 2;
			if (int.TryParse(adminId, out var userId))
			{
				var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
				if (employee != null)
				{
					invoice.EmployeeId = employee.Id;
				}
			}

			await _db.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
			return ServiceResult.Ok("Order confirmed successfully");
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			return ServiceResult.Fail("There was an error confirming the order");
		}
	}
}
