using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IAdminService
{
	Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default);
	Task<List<InvoiceDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
	Task<AdminProductsPageResult> GetProductsAsync(int pageNumber, int pageSize, int? categoryId, CancellationToken cancellationToken = default);
	Task<AdminClientsPageResult> GetClientsAsync(int pageNumber, int pageSize, int? role, CancellationToken cancellationToken = default);
	Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
