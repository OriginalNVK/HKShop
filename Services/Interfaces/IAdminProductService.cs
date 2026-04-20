using HKShop.DTOs;
using HKShop.Models;

namespace HKShop.Services.Interfaces;

public interface IAdminProductService
{
	Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
	Task<ServiceResult> CreateAsync(ProductRequestDto product, IFormFile? image, CancellationToken cancellationToken = default);
	Task<ServiceResult> UpdateAsync(int id, ProductRequestDto product, CancellationToken cancellationToken = default);
	Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
