using HKShop.Domain;
using HKShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Repositories;

public class CartRepository: ICartRepository
{
	private readonly HKShopDbContext _context;

	public CartRepository(HKShopDbContext context)
	{
		_context = context;
	}

	public async Task<List<DetailCart>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailCarts
			.AsNoTracking()
			.Include(d => d.Cart)
			.Include(d => d.Product)
			.Where(d => d.Cart.CustomerId == customerId)
			.OrderByDescending(d => d.AddedDate)
			.ToListAsync(cancellationToken);
	}

	public async Task<DetailCart?> GetItemAsync(int customerId, int productId, CancellationToken cancellationToken = default)
	{
		return await _context.DetailCarts
			.Include(d => d.Cart)
			.Include(d => d.Product)
			.FirstOrDefaultAsync(d => d.Cart.CustomerId == customerId && d.ProductId == productId, cancellationToken);
	}

	private async Task<Cart> GetOrCreateCartAsync(int customerId, CancellationToken cancellationToken)
	{
		var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
		if (cart != null)
		{
			return cart;
		}

		cart = new Cart
		{
			CustomerId = customerId,
			TotalPrice = 0m
		};
		await _context.Carts.AddAsync(cart, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return cart;
	}

	public async Task<DetailCart?> AddOrUpdateItemAsync(int customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (quantity <= 0)
		{
			return null;
		}

		var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
		if (product == null)
		{
			return null;
		}

		var cart = await GetOrCreateCartAsync(customerId, cancellationToken);
		var item = await _context.DetailCarts.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == productId, cancellationToken);

		var unitPrice = product.UnitPrice ?? 0m;
		if (item == null)
		{
			var detail = new DetailCart
			{
				CartId = cart.Id,
				ProductId = productId,
				Quantity = quantity,
				AddedDate = DateTime.UtcNow,
				SubPrice = unitPrice * quantity
			};
			await _context.DetailCarts.AddAsync(detail, cancellationToken);
			await RecalculateTotalAsync(cart.Id, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return detail;
		}
		else
		{
			item.Quantity += quantity;
			item.SubPrice = unitPrice * item.Quantity;
			item.AddedDate = DateTime.UtcNow;
		}

		await RecalculateTotalAsync(cart.Id, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return item;
	}

	private async Task RecalculateTotalAsync(int cartId, CancellationToken cancellationToken)
	{
		var total = await _context.DetailCarts
			.Where(d => d.CartId == cartId)
			.SumAsync(d => d.SubPrice, cancellationToken);

		var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
		if (cart != null)
		{
			cart.TotalPrice = total;
		}
	}

	public async Task<bool> UpdateQuantityAsync(int customerId, int productId, int quantity, CancellationToken cancellationToken = default)
	{
		var item = await GetItemAsync(customerId, productId, cancellationToken);

		if (item == null)
		{
			return false;
		}

		if (quantity <= 0)
		{
			_context.DetailCarts.Remove(item);
		}
		else
		{
			item.Quantity = quantity;
			item.SubPrice = (item.Product.UnitPrice ?? 0m) * quantity;
		}

		await RecalculateTotalAsync(item.CartId, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> RemoveItemAsync(int customerId, int productId, CancellationToken cancellationToken = default)
	{
		var item = await GetItemAsync(customerId, productId, cancellationToken);

		if (item == null)
		{
			return false;
		}

		_context.DetailCarts.Remove(item);
		await RecalculateTotalAsync(item.CartId, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<int> ClearCartAsync(int customerId, CancellationToken cancellationToken = default)
	{
		var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
		if (cart == null)
		{
			return 0;
		}

		var items = await _context.DetailCarts
			.Where(c => c.CartId == cart.Id)
			.ToListAsync(cancellationToken);

		if (items.Count == 0)
		{
			return 0;
		}

		_context.DetailCarts.RemoveRange(items);
		cart.TotalPrice = 0m;
		await _context.SaveChangesAsync(cancellationToken);
		return items.Count;
	}
}
