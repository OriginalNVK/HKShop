using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminProductService : IAdminProductService
{
	private readonly HKShopDbContext _db;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminProductService(HKShopDbContext db, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ProductsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _db.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ProductsResponse
		{
			ProductId = product.Id,
			ProductName = product.Name,
			AliasName = null,
			CategoryId = product.CategoryId,
			UnitDescription = product.UnitDescription,
			Price = product.UnitPrice,
			ImageUrl = product.Image,
			ManufactureDate = DateOnly.FromDateTime(product.CreatedDate),
			Discount = product.Discount,
			Views = product.Views,
			Description = product.Description,
			Category = product.Category
		};
	}

	public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		return await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(cancellationToken);
	}

	public async Task<ServiceResult> CreateAsync(ProductsRequest request, IFormFile? image, CancellationToken cancellationToken = default)
	{
		var product = new Product
		{
			Name = request.TenHh,
			CategoryId = request.MaLoai ?? 0,
			UnitDescription = request.MoTaDonVi,
			Description = request.MoTa,
			UnitPrice = request.DonGia,
			CreatedDate = request.NgaySx,
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
		var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		if (existingProduct == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		existingProduct.Name = request.TenHh;
		existingProduct.CategoryId = request.MaLoai ?? 0;
		existingProduct.UnitDescription = request.MoTaDonVi;
		existingProduct.Description = request.MoTa;
		existingProduct.UnitPrice = request.DonGia;
		existingProduct.CreatedDate = request.NgaySx;
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
