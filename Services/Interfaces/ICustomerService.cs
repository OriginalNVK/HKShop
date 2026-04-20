using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface ICustomerService
{
	Task<ServiceResult> RegisterAsync(RegisterRequestDto model, IFormFile? image, CancellationToken cancellationToken = default);
	Task<LoginResult> LoginAsync(LoginRequestDto model, string? returnUrl, CancellationToken cancellationToken = default);
}
