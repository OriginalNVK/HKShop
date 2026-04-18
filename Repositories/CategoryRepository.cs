using HKShop.Models;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class CategoryRepository : ICategoryRepository
{
	private readonly DBContext _context;

	public CategoryRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Categories
			.AsNoTracking()
			.OrderBy(c => c.CategoryName)
			.ToListAsync(cancellationToken);
	}

	public async Task<Category?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
	{
		return await _context.Categories
			.Include(c => c.Products)
			.FirstOrDefaultAsync(c => c.CategoryId == categoryId, cancellationToken);
	}

	public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
	{
		await _context.Categories.AddAsync(category, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return category;
	}

	public async Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default)
	{
		var existing = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId, cancellationToken);
		if (existing == null)
		{
			return false;
		}

		existing.CategoryName = category.CategoryName;
		existing.CategoryAlias = category.CategoryAlias;
		existing.Description = category.Description;
		existing.Image = category.Image;

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
	{
		var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId, cancellationToken);
		if (category == null)
		{
			return false;
		}

		_context.Categories.Remove(category);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<int> CountProductsAsync(int categoryId, CancellationToken cancellationToken = default)
	{
		return await _context.Products
			.AsNoTracking()
			.CountAsync(p => p.CategoryId == categoryId, cancellationToken);
	}
}
