using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IAdminCustomerService
{
	Task<ServiceResult> CreateUserAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default);
	Task<ClientResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<ServiceResult> UpdateAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default);
	Task<ServiceResult> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
