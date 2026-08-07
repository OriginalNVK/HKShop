using HKShop.DTOs;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class ProductService : IProductService
{
	private readonly HKShopDbContext _db;

	public ProductService(HKShopDbContext db)
	{
		_db = db;
	}

	public async Task<List<HangHoaResponse>> GetProductsAsync(int? categoryId, string? keyword, CancellationToken cancellationToken = default)
	{
		var products = _db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

		if (categoryId.HasValue && categoryId.Value != 0)
		{
			products = products.Where(p => p.CategoryId == categoryId.Value);
		}
		else if (!string.IsNullOrWhiteSpace(keyword))
		{
			products = products.Where(p => p.Name.Contains(keyword));
		}

		return await products
			.Select(p => new HangHoaResponse
			{
				MaHh = p.Id,
				TenHH = p.Name,
				DonGia = p.UnitPrice ?? 0,
				Hinh = p.Image ?? string.Empty,
				MoTaNgan = p.Description ?? string.Empty,
				TenLoai = p.Category.Name,
				GiamGia = p.Discount
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ChiTietHangHoaResponse?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _db.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ChiTietHangHoaResponse
		{
			MaHH = product.Id,
			TenHH = product.Name,
			DonGia = product.UnitPrice ?? 0,
			ChiTiet = product.Description ?? string.Empty,
			DiemDanhGia = 5,
			Hinh = product.Image ?? string.Empty,
			MoTaNgan = product.Description ?? string.Empty,
			TenLoai = product.Category.Name,
			SoLuongTon = 10
		};
	}
}
