using HKShop.DTOs;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminOrderService : IAdminOrderService
{
	private readonly DBContext _db;

	public AdminOrderService(DBContext db)
	{
		_db = db;
	}

	public async Task<List<DetailInvoiceVM>> GetDetailAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		return await _db.DetailInvoices
			.AsNoTracking()
			.Include(d => d.ProductIdNavigation)
			.Where(d => d.InvoiceId == invoiceId)
			.Select(d => new DetailInvoiceVM
			{
				MaCt = d.DetailInvoiceId,
				MaHd = d.InvoiceId,
				MaHh = d.ProductId,
				DonGia = d.Amount,
				SoLuong = d.Quantity,
				GiamGia = d.Discount,
				TenHangHoa = d.ProductIdNavigation.ProductName,
				Hinh = d.ProductIdNavigation.Image ?? string.Empty,
				ThanhTien = d.Amount * d.Quantity
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		var invoice = await _db.Invoices.FirstOrDefaultAsync(h => h.InvoiceId == invoiceId, cancellationToken);
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
			var invoice = await _db.Invoices.SingleOrDefaultAsync(h => h.InvoiceId == invoiceId, cancellationToken);
			if (invoice == null)
			{
				return ServiceResult.Fail("Order not found");
			}

			invoice.DeliveryDate = DateOnly.FromDateTime(deliveryDate);
			invoice.StatusCode = 2;
			invoice.AdminId = adminId;

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
