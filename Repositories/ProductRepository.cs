using HKShop.Models;
using Microsoft.EntityFrameworkCore;
using HKShop.Repositories.Interfaces;

namespace HKShop.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly DBContext _context;

	public ProductRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.OrderByDescending(p => p.ProductId)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Product>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null, string? keyword = null, CancellationToken cancellationToken = default)
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
			query = query.Where(p => p.ProductName.Contains(keyword) || (p.AliasName != null && p.AliasName.Contains(keyword)));
		}

		return await query
			.OrderByDescending(p => p.ProductId)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
	{
		return await _context.Products
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
	}

	public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
	{
		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return product;
	}

	public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.ProductName = product.ProductName;
		existing.AliasName = product.AliasName;
		existing.CategoryId = product.CategoryId;
		existing.Description = product.Description;
		existing.Price = product.Price;
		existing.Image = product.Image;
		existing.CreatedAt = product.CreatedAt;
		existing.Discount = product.Discount;
		existing.Views = product.Views;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int productId, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		_context.Products.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
