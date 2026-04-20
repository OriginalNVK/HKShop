using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IProductService
{
	Task<List<ProductResponseDto>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null, string? keyword = null, CancellationToken cancellationToken = default);
	Task<ProductDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
}
