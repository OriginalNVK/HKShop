using HKShop.DTOs;
using HKShop.Domain;

namespace HKShop.Services.Interfaces;

public interface IAdminProductService
{
	Task<ProductsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
	Task<ServiceResult> CreateAsync(ProductsRequest product, IFormFile? image, CancellationToken cancellationToken = default);
	Task<ServiceResult> UpdateAsync(int id, ProductsRequest product, CancellationToken cancellationToken = default);
	Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
