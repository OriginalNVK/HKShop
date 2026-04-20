using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class AdminProductService : IAdminProductService
{
	private readonly IProductRepository _productRepository;
	private readonly ICategoryRepository _categoryRepository;
	private readonly IDetailInvoiceRepository _detailInvoiceRepository;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminProductService(
		IProductRepository productRepository,
		ICategoryRepository categoryRepository,
		IDetailInvoiceRepository detailInvoiceRepository,
		ICloudinaryService cloudinaryService)
	{
		_productRepository = productRepository;
		_categoryRepository = categoryRepository;
		_detailInvoiceRepository = detailInvoiceRepository;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _productRepository.GetByIdAsync(id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ProductResponseDto
		{
			ProductId = product.ProductId,
			ProductName = product.ProductName,
			AliasName = product.AliasName,
			CategoryId = product.CategoryId,
			UnitDescription = product.Description,
			Price = product.Price,
			ImageUrl = product.Image,
			ManufactureDate = product.CreatedAt,
			Discount = product.Discount,
			Views = product.Views,
			Description = product.Description,
			Category = product.Category
		};
	}

	public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		return await _categoryRepository.GetAllAsync(cancellationToken);
	}

	public async Task<ServiceResult> CreateAsync(ProductRequestDto product, IFormFile? image, CancellationToken cancellationToken = default)
	{
		var newProduct = new Product
		{
			ProductName = product.ProductName,
			AliasName = product.AliasName,
			CategoryId = product.CategoryId ?? 0,
			Description = product.Description ?? product.UnitDescription,
			Price = product.Price,
			CreatedAt = DateOnly.FromDateTime(product.ManufactureDate),
			Discount = product.Discount ?? 0,
			Views = product.Views ?? 0
		};

		if (image != null && image.Length > 0)
		{
			try
			{
				newProduct.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_PRODUCT);
			}
			catch (Exception ex)
			{
				return ServiceResult.Fail("Upload image failed: " + ex.Message);
			}
		}

		await _productRepository.CreateAsync(newProduct, cancellationToken);
		return ServiceResult.Ok("Create product successfully");
	}

	public async Task<ServiceResult> UpdateAsync(int id, ProductRequestDto product, CancellationToken cancellationToken = default)
	{
		var existingProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
		if (existingProduct == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		existingProduct.ProductName = product.ProductName;
		existingProduct.AliasName = product.AliasName;
		existingProduct.CategoryId = product.CategoryId ?? 0;
		existingProduct.Description = product.Description ?? product.UnitDescription;
		existingProduct.Price = product.Price;
		existingProduct.CreatedAt = DateOnly.FromDateTime(product.ManufactureDate);
		existingProduct.Discount = product.Discount ?? 0;
		existingProduct.Views = product.Views ?? 0;

		if (product.ImageFile != null && product.ImageFile.Length > 0)
		{
			existingProduct.Image = await _cloudinaryService.UploadImageAsync(product.ImageFile, Constants.FOLDER_CLOUDINARY_PRODUCT);
		}

		await _productRepository.UpdateAsync(existingProduct, cancellationToken);
		return ServiceResult.Ok("Update product successfully");
	}

	public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var detailExists = await _detailInvoiceRepository.ExistsByProductIdAsync(id, cancellationToken);
		if (detailExists)
		{
			return ServiceResult.Fail("Cannot delete this product because it is used in invoices");
		}

		var product = await _productRepository.GetByIdAsync(id, cancellationToken);
		if (product == null)
		{
			return ServiceResult.Fail("Product not found");
		}

		await _productRepository.DeleteAsync(id, cancellationToken);
		return ServiceResult.Ok("Delete product successfully");
	}
}
