using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IProductService
{
	Task<List<HangHoaResponse>> GetProductsAsync(int? categoryId, string? keyword, CancellationToken cancellationToken = default);
	Task<ChiTietHangHoaResponse?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
}
