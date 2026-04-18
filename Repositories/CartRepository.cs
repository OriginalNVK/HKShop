using HKShop.Models;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class CartRepository: ICartRepository
{
	private readonly DBContext _context;

	public CartRepository(DBContext context)
	{
		_context = context;
	}

	public async Task<List<Cart>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
	{
		return await _context.Carts
			.AsNoTracking()
			.Include(c => c.ProductIdNavigation)
			.Where(c => c.CustomerId == customerId)
			.OrderByDescending(c => c.AddedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task<Cart?> GetItemAsync(string customerId, int productId, CancellationToken cancellationToken = default)
	{
		return await _context.Carts
			.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);
	}

	public async Task<Cart?> AddOrUpdateItemAsync(string customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (quantity <= 0)
		{
			return null;
		}

		var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
		if (product == null)
		{
			return null;
		}

		var item = await _context.Carts
			.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);

		var unitPrice = (product.Price ?? 0m) * (1m - product.Discount / 100m);
		if (item == null)
		{
			item = new Cart
			{
				CustomerId = customerId,
				ProductId = productId,
				Quantity = quantity,
				Amount = unitPrice,
				AddedAt = DateTime.UtcNow
			};

			await _context.Carts.AddAsync(item, cancellationToken);
		}
		else
		{
			item.Quantity += quantity;
			item.Amount = unitPrice;
			item.AddedAt = DateTime.UtcNow;
		}

		await _context.SaveChangesAsync(cancellationToken);
		return item;
	}

	public async Task<bool> UpdateQuantityAsync(string customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		var item = await _context.Carts
			.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);

		if (item == null)
		{
			return false;
		}

		if (quantity <= 0)
		{
			_context.Carts.Remove(item);
		}
		else
		{
			item.Quantity = quantity;
		}

		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> RemoveItemAsync(string customerId, int productId, CancellationToken cancellationToken = default)
	{
		var item = await _context.Carts
			.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId, cancellationToken);

		if (item == null)
		{
			return false;
		}

		_context.Carts.Remove(item);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<int> ClearCartAsync(string customerId, CancellationToken cancellationToken = default)
	{
		var items = await _context.Carts
			.Where(c => c.CustomerId == customerId)
			.ToListAsync(cancellationToken);

		if (items.Count == 0)
		{
			return 0;
		}

		_context.Carts.RemoveRange(items);
		await _context.SaveChangesAsync(cancellationToken);
		return items.Count;
	}

	public async Task<decimal> GetCartTotalAsync(string customerId, CancellationToken cancellationToken = default)
	{
		return await _context.Carts
			.AsNoTracking()
			.Where(c => c.CustomerId == customerId)
			.SumAsync(c => c.Amount * c.Quantity, cancellationToken);
	}
}
