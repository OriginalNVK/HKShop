using HKShop.DTOs;
using HKShop.Models;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class ProductService : IProductService
{
	private readonly IProductRepository _product;

	public ProductService(IProductRepository product)
	{
		_product = product;
	}

	public async Task<List<ProductResponseDto>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null, string? keyword = null, CancellationToken cancellationToken = default)
	{
		var products = await _product.GetPagedAsync(pageNumber, pageSize, categoryId, keyword, cancellationToken);

		return products.Select(p => new ProductResponseDto
		{
			ProductId = p.ProductId,
			ProductName = p.ProductName,
			Price = p.Price ?? 0,
			Description = p.Description ?? string.Empty,
			ImageUrl = p.Image ?? string.Empty,
			Category = p.Category
		}).ToList();
	}

	public async Task<ProductDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
	{
		var product = await _product.GetByIdAsync(id, cancellationToken);

		if (product == null)
		{
			return null;
		}

		return new ProductDetailDto
		{
			ProductId = product.ProductId,
			ProductName = product.ProductName,
			Price = product.Price ?? 0,
			Description = product.Description ?? string.Empty,
			ImageUrl = product.Image ?? string.Empty,
			CategoryName = product.Category.CategoryName ?? string.Empty
		};
	}
}