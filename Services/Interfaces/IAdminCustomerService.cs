using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IAdminCustomerService
{
	Task<ServiceResult> CreateUserAsync(CustomerRequestDto client, IFormFile? image, CancellationToken cancellationToken = default);
	Task<CustomerResponseDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<ServiceResult> UpdateAsync(CustomerRequestDto client, IFormFile? image, CancellationToken cancellationToken = default);
	Task<ServiceResult> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
