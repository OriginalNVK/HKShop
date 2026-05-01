using HKShop.DTOs;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class ProductService : IProductService
{
	private readonly DBContext _db;

	public ProductService(DBContext db)
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
			products = products.Where(p => p.ProductName.Contains(keyword));
		}

		return await products
			.Select(p => new HangHoaResponse
			{
				MaHh = p.ProductId,
				TenHH = p.ProductName,
				DonGia = p.Price ?? 0,
				Hinh = p.Image ?? string.Empty,
				MoTaNgan = p.Description ?? string.Empty,
				TenLoai = p.Category.CategoryName,
				GiamGia = p.Discount
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<ChiTietHangHoaResponse?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _db.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.SingleOrDefaultAsync(p => p.ProductId == id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ChiTietHangHoaResponse
		{
			MaHH = product.ProductId,
			TenHH = product.ProductName,
			DonGia = product.Price ?? 0,
			ChiTiet = product.Description ?? string.Empty,
			DiemDanhGia = 5,
			Hinh = product.Image ?? string.Empty,
			MoTaNgan = product.Description ?? string.Empty,
			TenLoai = product.Category.CategoryName,
			SoLuongTon = 10
		};
	}
}
