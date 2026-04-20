using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class CartService : ICartService
{
	private readonly ICartRepository _cart;
	private readonly IProductRepository _product;
	private readonly ICustomerRepository _customer;
	private readonly IInvoiceRepository _invoiceRepository;
	private readonly IDetailInvoiceRepository _detailInvoiceRepository;
	private readonly PaypalClient _paypalClient;

	public CartService(
		ICartRepository cart,
		IProductRepository product,
		ICustomerRepository customer,
		IInvoiceRepository invoiceRepository,
		IDetailInvoiceRepository detailInvoiceRepository,
		PaypalClient paypalClient)
	{
		_cart = cart;
		_product = product;
		_customer = customer;
		_invoiceRepository = invoiceRepository;
		_detailInvoiceRepository = detailInvoiceRepository;
		_paypalClient = paypalClient;
	}

	public async Task<List<CartItemDto>?> GetCartAsync(string? customerId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return null;
		}

		var items = await _cart.GetByCustomerIdAsync(customerId, cancellationToken);
		return items.Select(c => new CartItemDto
		{
			ProductId = c.ProductId,
			ProductName = c.ProductIdNavigation.ProductName,
			Price = c.Amount,
			Quantity = c.Quantity,
			ImageUrl = c.ProductIdNavigation.Image ?? string.Empty
		}).ToList();
	}

	public async Task<ServiceResult> AddToCartAsync(string? customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return ServiceResult.Fail("Unauthorized");
		}

		var product = await _product.GetByIdAsync(productId, cancellationToken);
		if (product == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		var item = await _cart.AddOrUpdateItemAsync(customerId, productId, quantity, cancellationToken);
		if (item == null)
		{
			return ServiceResult.Fail("Cannot add product to cart");
		}

		return ServiceResult.Ok();
	}
		
	public async Task RemoveCartItemAsync(string? customerId, int productId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(customerId))
		{
			return;
		}

		await _cart.RemoveItemAsync(customerId, productId, cancellationToken);
	}

	public async Task<List<CartItemDto>> GetCheckoutItemsAsync(string customerId, CancellationToken cancellationToken = default)
	{
		var carts = await _cart.GetByCustomerIdAsync(customerId, cancellationToken);
		return carts.Select(c => new CartItemDto
			{
				ProductId = c.ProductId,
				ProductName = c.ProductIdNavigation.ProductName,
				Price = c.Amount,
				Quantity = c.Quantity,
				ImageUrl = c.ProductIdNavigation.Image ?? string.Empty
			})
			.ToList();
	}

	public async Task<ServiceResult> CheckoutCodAsync(string customerId, CheckoutRequestDto model, CancellationToken cancellationToken = default)
	{
		var carts = await _cart.GetByCustomerIdAsync(customerId, cancellationToken);
		if (carts.Count == 0)
		{
			return ServiceResult.Fail("Cart is empty");
		}

		var customer = model.UseCustomerProfile
			? await _customer.GetByUsernameAsync(customerId, cancellationToken)
			: null;

		var invoice = new Invoice
		{
			CustomerId = customerId,
			CustomerName = model.FullName ?? customer?.FullName,
			Address = model.Address ?? customer?.Address ?? string.Empty,
			PhoneNumber = model.PhoneNumber ?? customer?.PhoneNumber ?? string.Empty,
			OrderDate = DateTime.Now,
			PaymentMethod = "COD",
			ShippingMethod = "Grab",
			StatusCode = 0,
			Notes = model.Notes
		};

		try
		{
			await _invoiceRepository.CreateAsync(invoice, cancellationToken);

			var details = carts.Select(item => new DetailInvoice
			{
				InvoiceId = invoice.InvoiceId,
				Quantity = item.Quantity,
				Amount = item.Amount,
				ProductId = item.ProductId,
				Discount = 0
			}).ToList();

			foreach (var detail in details)
			{
				await _detailInvoiceRepository.CreateAsync(detail, cancellationToken);
			}

			await _cart.ClearCartAsync(customerId, cancellationToken);
			return ServiceResult.Ok("Checkout success");
		}
		catch
		{
			return ServiceResult.Fail("Checkout failed");
		}
	}

	public async Task<CreateOrderResponse> CreatePaypalOrderAsync(string customerId, CancellationToken cancellationToken = default)
	{
		var cartItems = await GetCheckoutItemsAsync(customerId, cancellationToken);
		var total = cartItems.Sum(p => p.LineTotal).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
		var reference = "INV" + DateTime.Now.Ticks;
		return await _paypalClient.CreateOrder(total, "USD", reference);
	}

	public async Task<PaypalCaptureResult> CapturePaypalOrderAsync(string customerId, string orderId, CancellationToken cancellationToken = default)
	{
		var carts = await _cart.GetByCustomerIdAsync(customerId, cancellationToken);
		if (carts.Count == 0)
		{
			return new PaypalCaptureResult { Success = false, Message = "Cart is empty" };
		}

		try
		{
			var response = await _paypalClient.CaptureOrder(orderId);
			var customer = await _customer.GetByUsernameAsync(customerId, cancellationToken);

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

			try
			{
				await _invoiceRepository.CreateAsync(invoice, cancellationToken);

				var details = carts.Select(item => new DetailInvoice
				{
					InvoiceId = invoice.InvoiceId,
					Quantity = item.Quantity,
					Amount = item.Amount,
					ProductId = item.ProductId,
					Discount = 0
				}).ToList();

				foreach (var detail in details)
				{
					await _detailInvoiceRepository.CreateAsync(detail, cancellationToken);
				}

				await _cart.ClearCartAsync(customerId, cancellationToken);

				return new PaypalCaptureResult { Success = true, Data = response, Message = "Payment success" };
			}
			catch (Exception ex)
			{
				return new PaypalCaptureResult { Success = false, Message = "Failed to save invoice: " + ex.Message };
			}
		}
		catch (Exception ex)
		{
			return new PaypalCaptureResult { Success = false, Message = ex.GetBaseException().Message };
		}
	}
}
