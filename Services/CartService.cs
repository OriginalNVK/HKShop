using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class CartService : ICartService
{
	private readonly HKShopDbContext _db;
	private readonly PaypalClient _paypalClient;

	public CartService(HKShopDbContext db, PaypalClient paypalClient)
	{
		_db = db;
		_paypalClient = paypalClient;
	}

	private static bool TryParseCustomerId(string? customerId, out int parsedCustomerId)
	{
		return int.TryParse(customerId, out parsedCustomerId);
	}

	public async Task<List<GioHangItem>?> GetCartAsync(string? customerId, CancellationToken cancellationToken = default)
	{
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return null;
		}

		return await _db.DetailCarts
			.AsNoTracking()
			.Include(c => c.Cart)
			.Include(c => c.Product)
			.Where(c => c.Cart.CustomerId == customerInt)
			.Select(c => new GioHangItem
			{
				ProductId = c.ProductId,
				ProductName = c.Product.Name,
				Price = c.Product.UnitPrice ?? 0,
				Quantity = c.Quantity,
				ImageUrl = c.Product.Image ?? string.Empty
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> AddToCartAsync(string? customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return ServiceResult.Fail("Unauthorized");
		}

		var cart = await _db.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerInt, cancellationToken);
		if (cart == null)
		{
			cart = new Cart { CustomerId = customerInt, TotalPrice = 0m };
			await _db.Carts.AddAsync(cart, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);
		}

		var product = await _db.Products.SingleOrDefaultAsync(p => p.Id == productId, cancellationToken);
		if (product == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		var item = await _db.DetailCarts.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == productId, cancellationToken);
		if (item == null)
			{
			item = new DetailCart
			{
				CartId = cart.Id,
				ProductId = product.Id,
				Quantity = quantity,
				AddedDate = DateTime.Now,
				SubPrice = (product.UnitPrice ?? 0) * quantity
			};
			await _db.DetailCarts.AddAsync(item, cancellationToken);
		}
		else
		{
			item.Quantity += quantity;
			item.SubPrice = (product.UnitPrice ?? 0) * item.Quantity;
		}

		cart.TotalPrice = await _db.DetailCarts.Where(d => d.CartId == cart.Id).SumAsync(d => d.SubPrice, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok();
	}

	public async Task RemoveCartItemAsync(string? customerId, int productId, CancellationToken cancellationToken = default)
	{
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return;
		}

		var cart = await _db.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerInt, cancellationToken);
		if (cart == null)
		{
			return;
		}

		var item = await _db.DetailCarts.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == productId, cancellationToken);
		if (item != null)
		{
			_db.DetailCarts.Remove(item);
			cart.TotalPrice = await _db.DetailCarts.Where(d => d.CartId == cart.Id).SumAsync(d => d.SubPrice, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);
		}
	}

	public async Task<List<GioHangItem>> GetCheckoutItemsAsync(string customerId, CancellationToken cancellationToken = default)
	{
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return new List<GioHangItem>();
		}

		return await _db.DetailCarts
			.AsNoTracking()
			.Include(c => c.Cart)
			.Include(c => c.Product)
			.Where(c => c.Cart.CustomerId == customerInt)
			.Select(c => new GioHangItem
			{
				ProductId = c.ProductId,
				ProductName = c.Product.Name,
				Price = c.Product.UnitPrice ?? 0,
				Quantity = c.Quantity,
				ImageUrl = c.Product.Image ?? string.Empty
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> CheckoutCodAsync(string customerId, CheckoutVM model, CancellationToken cancellationToken = default)
	{
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return ServiceResult.Fail("Unauthorized");
		}

		var carts = await _db.DetailCarts.Include(c => c.Cart).Where(c => c.Cart.CustomerId == customerInt).ToListAsync(cancellationToken);
		if (carts.Count == 0)
		{
			return ServiceResult.Fail("Cart is empty");
		}

		var customer = model.GiongKhachHang
			? await _db.Customers.SingleOrDefaultAsync(c => c.Id == customerInt, cancellationToken)
			: null;

		var invoice = new Invoice
		{
			CustomerId = customerInt,
			ReceiverName = model.HoTen ?? customer?.Fullname,
			Address = model.DiaChi ?? customer?.Address ?? string.Empty,
			PhoneNumber = model.DienThoai ?? customer?.Phone ?? string.Empty,
			OrderDate = DateTime.Now,
			PaymentMethod = "COD",
			ShippingMethod = "Grab",
			StatusId = 0,
			Note = model.GhiChu,
			ShippingFee = 0m,
			Discount = 0m,
			TotalPrice = 0m
		};

		await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			await _db.Invoices.AddAsync(invoice, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);

			var details = carts.Select(item => new DetailInvoice
			{
				InvoiceId = invoice.Id,
				Quantity = item.Quantity,
				SubPrice = item.SubPrice,
				ProductId = item.ProductId,
			}).ToList();

			await _db.DetailInvoices.AddRangeAsync(details, cancellationToken);
			_db.DetailCarts.RemoveRange(carts);
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
		if (!TryParseCustomerId(customerId, out var customerInt))
		{
			return new PaypalCaptureResult { Success = false, Message = "Unauthorized" };
		}

		var carts = await _db.DetailCarts.Include(c => c.Cart).Where(c => c.Cart.CustomerId == customerInt).ToListAsync(cancellationToken);
		if (carts.Count == 0)
		{
			return new PaypalCaptureResult { Success = false, Message = "Cart is empty" };
		}

		try
		{
			var response = await _paypalClient.CaptureOrder(orderId);
			var customer = await _db.Customers.SingleOrDefaultAsync(c => c.Id == customerInt, cancellationToken);

			var invoice = new Invoice
			{
				CustomerId = customerInt,
				ReceiverName = customer?.Fullname ?? response.payer.name.given_name,
				Address = customer?.Address ?? "N/A",
				PhoneNumber = customer?.Phone ?? "N/A",
				OrderDate = DateTime.Now,
				PaymentMethod = "PayPal",
				ShippingMethod = "Grab",
				StatusId = 1,
				Note = "Paid with PayPal",
				ShippingFee = 0m,
				Discount = 0m,
				TotalPrice = 0m
			};

			await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
			try
			{
				await _db.Invoices.AddAsync(invoice, cancellationToken);
				await _db.SaveChangesAsync(cancellationToken);

				var details = carts.Select(item => new DetailInvoice
				{
					InvoiceId = invoice.Id,
					Quantity = item.Quantity,
					SubPrice = item.SubPrice,
					ProductId = item.ProductId,
				}).ToList();

				await _db.DetailInvoices.AddRangeAsync(details, cancellationToken);
				_db.DetailCarts.RemoveRange(carts);
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
