using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminProductService : IAdminProductService
{
	private readonly DBContext _db;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminProductService(DBContext db, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ProductsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _db.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ProductsResponse
		{
			MaHh = product.ProductId,
			TenHh = product.ProductName,
			TenAlias = product.AliasName,
			MaLoai = product.CategoryId,
			MoTaDonVi = product.Description,
			DonGia = product.Price,
			Hinh = product.Image,
			NgaySx = product.CreatedAt,
			GiamGia = product.Discount,
			LuotMua = product.Views,
			MoTa = product.Description,
			MaLoaiNavigation = product.Category
		};
	}

	public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		return await _db.Categories.AsNoTracking().OrderBy(c => c.CategoryName).ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> CreateAsync(ProductsRequest request, IFormFile? image, CancellationToken cancellationToken = default)
	{
		var product = new Product
		{
			ProductName = request.TenHh,
			AliasName = request.TenAlias,
			CategoryId = request.MaLoai ?? 0,
			Description = request.MoTa ?? request.MoTaDonVi,
			Price = request.DonGia,
			CreatedAt = DateOnly.FromDateTime(request.NgaySx),
			Discount = request.GiamGia ?? 0,
			Views = request.LuotMua ?? 0
		};

		if (image != null && image.Length > 0)
		{
			try
			{
				product.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_PRODUCT);
			}
			catch (Exception ex)
			{
				return ServiceResult.Fail("Upload image failed: " + ex.Message);
			}
		}

		await _db.Products.AddAsync(product, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Create product successfully");
	}

	public async Task<ServiceResult> UpdateAsync(int id, ProductsRequest request, CancellationToken cancellationToken = default)
	{
		var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
		if (existingProduct == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		existingProduct.ProductName = request.TenHh;
		existingProduct.AliasName = request.TenAlias;
		existingProduct.CategoryId = request.MaLoai ?? 0;
		existingProduct.Description = request.MoTa ?? request.MoTaDonVi;
		existingProduct.Price = request.DonGia;
		existingProduct.CreatedAt = DateOnly.FromDateTime(request.NgaySx);
		existingProduct.Discount = request.GiamGia ?? 0;
		existingProduct.Views = request.LuotMua ?? 0;

		if (request.Hinh != null && request.Hinh.Length > 0)
		{
			existingProduct.Image = await _cloudinaryService.UploadImageAsync(request.Hinh, Constants.FOLDER_CLOUDINARY_PRODUCT);
		}

		_db.Products.Update(existingProduct);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Update product successfully");
	}

	public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var detailExists = await _db.DetailInvoices.AnyAsync(c => c.ProductId == id, cancellationToken);
		if (detailExists)
		{
			return ServiceResult.Fail("Cannot delete this product because it is used in invoices");
		}

		var product = await _db.Products.FindAsync(new object[] { id }, cancellationToken);
		if (product == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		_db.Products.Remove(product);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Delete product successfully");
	}
}
