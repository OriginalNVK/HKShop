using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class CartService : ICartService
{
	private readonly DBContext _db;
	private readonly PaypalClient _paypalClient;

	public CartService(DBContext db, PaypalClient paypalClient)
	{
		_db = db;
		_paypalClient = paypalClient;
	}

	public async Task<List<GioHangItem>?> GetCartAsync(string? customerId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return null;
		}

		return await _db.Carts
			.AsNoTracking()
			.Include(c => c.ProductIdNavigation)
			.Where(c => c.CustomerId == customerId)
			.Select(c => new GioHangItem
			{
				MaHH = c.ProductId,
				TenHH = c.ProductIdNavigation.ProductName,
				DonGia = c.Amount,
				SoLuong = c.Quantity,
				Hinh = c.ProductIdNavigation.Image ?? string.Empty
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> AddToCartAsync(string? customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return ServiceResult.Fail("Unauthorized");
		}

		var item = await _db.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);
		if (item == null)
		{
			var product = await _db.Products.SingleOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
			if (product == null)
			{
				return ServiceResult.Fail("Product not found");
			}

			item = new Cart
			{
				CustomerId = customerId,
				ProductId = product.ProductId,
				Amount = product.Price ?? 0,
				Quantity = quantity,
				AddedAt = DateTime.Now
			};
			await _db.Carts.AddAsync(item, cancellationToken);
		}
		else
		{
			item.Quantity += quantity;
			_db.Carts.Update(item);
		}

		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok();
	}

	public async Task RemoveCartItemAsync(string? customerId, int productId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return;
		}

		var item = await _db.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);
		if (item != null)
		{
			_db.Carts.Remove(item);
			await _db.SaveChangesAsync(cancellationToken);
		}
	}

	public async Task<List<GioHangItem>> GetCheckoutItemsAsync(string customerId, CancellationToken cancellationToken = default)
	{
		return await _db.Carts
			.AsNoTracking()
			.Include(c => c.ProductIdNavigation)
			.Where(c => c.CustomerId == customerId)
			.Select(c => new GioHangItem
			{
				MaHH = c.ProductId,
				TenHH = c.ProductIdNavigation.ProductName,
				DonGia = c.Amount,
				SoLuong = c.Quantity,
				Hinh = c.ProductIdNavigation.Image ?? string.Empty
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> CheckoutCodAsync(string customerId, CheckoutVM model, CancellationToken cancellationToken = default)
	{
		var carts = await _db.Carts.Where(c => c.CustomerId == customerId).ToListAsync(cancellationToken);
		if (carts.Count == 0)
		{
			return ServiceResult.Fail("Cart is empty");
		}

		var customer = model.GiongKhachHang
			? await _db.Customers.SingleOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken)
			: null;

		var invoice = new Invoice
		{
			CustomerId = customerId,
			CustomerName = model.HoTen ?? customer?.FullName,
			Address = model.DiaChi ?? customer?.Address ?? string.Empty,
			PhoneNumber = model.DienThoai ?? customer?.PhoneNumber ?? string.Empty,
			OrderDate = DateTime.Now,
			PaymentMethod = "COD",
			ShippingMethod = "Grab",
			StatusCode = 0,
			Notes = model.GhiChu
		};

		await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			await _db.Invoices.AddAsync(invoice, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);

			var details = carts.Select(item => new DetailInvoice
			{
				InvoiceId = invoice.InvoiceId,
				Quantity = item.Quantity,
				Amount = item.Amount,
				ProductId = item.ProductId,
				Discount = 0
			}).ToList();

			await _db.DetailInvoices.AddRangeAsync(details, cancellationToken);
			_db.Carts.RemoveRange(carts);
			await _db.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
			return ServiceResult.Ok("Checkout success");
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			return ServiceResult.Fail("Checkout failed");
		}
	}

	public async Task<CreateOrderResponse> CreatePaypalOrderAsync(string customerId, CancellationToken cancellationToken = default)
	{
		var cartItems = await GetCheckoutItemsAsync(customerId, cancellationToken);
		var total = cartItems.Sum(p => p.ThanhTien).ToString();
		var reference = "INV" + DateTime.Now.Ticks;
		return await _paypalClient.CreateOrder(total, "USD", reference);
	}

	public async Task<PaypalCaptureResult> CapturePaypalOrderAsync(string customerId, string orderId, CancellationToken cancellationToken = default)
	{
		var carts = await _db.Carts.Where(c => c.CustomerId == customerId).ToListAsync(cancellationToken);
		if (carts.Count == 0)
		{
			return new PaypalCaptureResult { Success = false, Message = "Cart is empty" };
		}

		try
		{
			var response = await _paypalClient.CaptureOrder(orderId);
			var customer = await _db.Customers.SingleOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

			var invoice = new Invoice
			{
				CustomerId = customerId,
				CustomerName = customer?.FullName ?? response.payer.name.given_name,
				Address = customer?.Address ?? "N/A",
				PhoneNumber = customer?.PhoneNumber ?? "N/A",
				OrderDate = DateTime.Now,
				PaymentMethod = "PayPal",
				ShippingMethod = "Grab",
				StatusCode = 1,
				Notes = "Paid with PayPal"
			};

			await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
			try
			{
				await _db.Invoices.AddAsync(invoice, cancellationToken);
				await _db.SaveChangesAsync(cancellationToken);

				var details = carts.Select(item => new DetailInvoice
				{
					InvoiceId = invoice.InvoiceId,
					Quantity = item.Quantity,
					Amount = item.Amount,
					ProductId = item.ProductId,
					Discount = 0
				}).ToList();

				await _db.DetailInvoices.AddRangeAsync(details, cancellationToken);
				_db.Carts.RemoveRange(carts);
				await _db.SaveChangesAsync(cancellationToken);
				await tx.CommitAsync(cancellationToken);

				return new PaypalCaptureResult { Success = true, Data = response, Message = "Payment success" };
			}
			catch (Exception ex)
			{
				await tx.RollbackAsync(cancellationToken);
				return new PaypalCaptureResult { Success = false, Message = "Failed to save invoice: " + ex.Message };
			}
		}
		catch (Exception ex)
		{
			return new PaypalCaptureResult { Success = false, Message = ex.GetBaseException().Message };
		}
	}
}
