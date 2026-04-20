using HKShop.DTOs;
using HKShop.Helpers;

namespace HKShop.Services.Interfaces;

public interface ICartService
{
	Task<List<CartItemDto>?> GetCartAsync(string? customerId, CancellationToken cancellationToken = default);
	Task<ServiceResult> AddToCartAsync(string? customerId, int productId, int quantity, CancellationToken cancellationToken = default);
	Task RemoveCartItemAsync(string? customerId, int productId, CancellationToken cancellationToken = default);
	Task<List<CartItemDto>> GetCheckoutItemsAsync(string customerId, CancellationToken cancellationToken = default);
	Task<ServiceResult> CheckoutCodAsync(string customerId, CheckoutRequestDto model, CancellationToken cancellationToken = default);
	Task<CreateOrderResponse> CreatePaypalOrderAsync(string customerId, CancellationToken cancellationToken = default);
	Task<PaypalCaptureResult> CapturePaypalOrderAsync(string customerId, string orderId, CancellationToken cancellationToken = default);
}
