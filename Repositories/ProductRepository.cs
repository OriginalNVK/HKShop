using HKShop.Domain;
using Microsoft.EntityFrameworkCore;
using HKShop.Repositories.Interfaces;

namespace HKShop.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly HKShopDbContext _context;

	public ProductRepository(HKShopDbContext context)
	{
		_context = context;
	}

	public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.OrderByDescending(p => p.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize, int? categoryId = null, string? keyword = null, CancellationToken cancellationToken = default)
	{
		var query = _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.AsQueryable();

		if (categoryId.HasValue)
		{
			query = query.Where(p => p.CategoryId == categoryId.Value);
		}

		if (!string.IsNullOrWhiteSpace(keyword))
		{
			query = query.Where(p => p.Name.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)));
		}

		return await query
			.OrderByDescending(p => p.Id)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
	{
		return await _context.Products
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
	}

	public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
	{
		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return product;
	}

	public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.Name = product.Name;
		existing.CategoryId = product.CategoryId;
		existing.UnitDescription = product.UnitDescription;
		existing.Description = product.Description;
		existing.UnitPrice = product.UnitPrice;
		existing.Image = product.Image;
		existing.CreatedDate = product.CreatedDate;
		existing.Discount = product.Discount;
		existing.Views = product.Views;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int productId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.Products.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
