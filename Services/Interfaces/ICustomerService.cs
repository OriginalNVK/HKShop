using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface ICustomerService
{
	Task<ServiceResult> RegisterAsync(DangKyRequest model, IFormFile? image, CancellationToken cancellationToken = default);
	Task<LoginResult> LoginAsync(DangNhapRequest model, string? returnUrl, CancellationToken cancellationToken = default);
}
