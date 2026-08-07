using HKShop.DTOs;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminService : IAdminService
{
	private readonly HKShopDbContext _db;

	public AdminService(HKShopDbContext db)
	{
		_db = db;
	}

	public async Task<OverviewDTO> GetOverviewAsync(CancellationToken cancellationToken = default)
	{
		var endDate = DateTime.Today;
		var startCustomer = endDate.AddDays(-6);
		var startOrder = endDate.AddDays(-13);

		var customerRaw = await _db.AppUsers
			.Where(u => u.Role == 0 && u.CreatedDate.Date >= startCustomer && u.CreatedDate.Date <= endDate)
			.GroupBy(u => u.CreatedDate.Date)
			.Select(g => new { Date = g.Key, Amount = g.Count() })
			.ToListAsync(cancellationToken);

		var orderRaw = await _db.Invoices
			.Where(i => i.OrderDate.Date >= startOrder && i.OrderDate.Date <= endDate)
			.GroupBy(i => i.OrderDate.Date)
			.Select(g => new { Date = g.Key, Amount = g.Count() })
			.ToListAsync(cancellationToken);

		var customerMap = customerRaw.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Amount);
		var orderMap = orderRaw.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Amount);

		var result = new OverviewDTO();
		for (var i = 0; i < 7; i++)
		{
			var d = DateOnly.FromDateTime(startCustomer.AddDays(i));
			result.CustomerIn7Day.Add(new DailyMetricDto
			{
				Date = d,
				Amount = customerMap.GetValueOrDefault(d, 0)
			});
		}

		for (var i = 0; i < 14; i++)
		{
			var d = DateOnly.FromDateTime(startOrder.AddDays(i));
			result.OrderIn14Day.Add(new DailyMetricDto
			{
				Date = d,
				Amount = orderMap.GetValueOrDefault(d, 0)
			});
		}

		return result;
	}

	public async Task<List<InvoiceResponse>> GetOrdersAsync(CancellationToken cancellationToken = default)
	{
		return await _db.Invoices
			.OrderByDescending(i => i.OrderDate)
			.Select(i => new InvoiceResponse
			{
				InvoiceId = i.Id,
				CustomerName = i.ReceiverName ?? string.Empty,
				NgayDat = i.OrderDate,
				DiaChi = i.Address,
				PaymentMethod = i.PaymentMethod,
				ShippingMethod = i.ShippingMethod,
				Status = MapInvoiceStatus(i.StatusId),
				Notes = i.Note ?? string.Empty,
				PhoneNumber = i.PhoneNumber
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<AdminProductsPageResult> GetProductsAsync(int pageNumber, int pageSize, int? categoryId, CancellationToken cancellationToken = default)
	{
		pageNumber = Math.Max(1, pageNumber);
		pageSize = Math.Max(1, pageSize);

		var query = _db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();
		if (categoryId.HasValue && categoryId.Value != 0)
		{
			query = query.Where(p => p.CategoryId == categoryId.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var products = await query
			.OrderByDescending(p => p.Id)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(p => new HangHoaResponse
			{
				ProductId = p.Id,
				ProductName = p.Name,
				Price = p.UnitPrice ?? 0,
				ImageUrl = p.Image ?? string.Empty,
				ShortDescription = p.Description ?? string.Empty,
				CategoryName = p.Category.Name,
				Discount = p.Discount
			})
			.ToListAsync(cancellationToken);

		var categories = await _db.Categories
			.AsNoTracking()
			.OrderBy(c => c.Name)
			.Select(c => new CategoryResponse
			{
				CategoryId = c.Id,
				CategoryName = c.Name,
				CategoryAlias = null,
				Description = c.Description,
				ImageUrl = c.Image
			})
			.ToListAsync(cancellationToken);

		return new AdminProductsPageResult
		{
			Products = products,
			Categories = categories,
			TotalPages = totalPages == 0 ? 1 : totalPages
		};
	}

	public async Task<AdminClientsPageResult> GetClientsAsync(int pageNumber, int pageSize, int? role, CancellationToken cancellationToken = default)
	{
		pageNumber = Math.Max(1, pageNumber);
		pageSize = Math.Max(1, pageSize);

		var query = _db.Customers.AsNoTracking().Include(c => c.User).AsQueryable();
		if (role.HasValue)
		{
			query = query.Where(c => c.User.Role == role.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var clients = await query
			.OrderBy(c => c.Fullname)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(c => new ClientResponse
			{
				CustomerId = c.Id.ToString(),
				FullName = c.Fullname,
				Gender = c.Gender,
				BirthDate = c.Birthday,
				Address = c.Address,
				PhoneNumber = c.Phone,
				Email = c.Email,
				Role = c.User.Role,
				ImageUrl = c.Avatar
			})
			.ToListAsync(cancellationToken);

		var roles = await _db.AppUsers
			.AsNoTracking()
			.Select(u => u.Role)
			.Distinct()
			.OrderBy(x => x)
			.ToListAsync(cancellationToken);

		return new AdminClientsPageResult
		{
			Clients = clients,
			Roles = roles,
			TotalCount = totalCount,
			TotalPages = totalPages == 0 ? 1 : totalPages
		};
	}

	public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		return await _db.Categories
			.AsNoTracking()
			.OrderBy(c => c.Name)
			.Select(c => new CategoryResponse
			{
				CategoryId = c.Id,
				CategoryName = c.Name,
				CategoryAlias = null,
				Description = c.Description,
				ImageUrl = c.Image
			})
			.ToListAsync(cancellationToken);
	}

	private static string MapInvoiceStatus(int statusCode)
	{
		return statusCode switch
		{
			0 => "pending",
			1 => "paid",
			2 => "processing",
			3 => "completed",
			4 => "cancelled",
			_ => "pending"
		};
	}
}
