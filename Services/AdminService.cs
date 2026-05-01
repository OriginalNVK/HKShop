using HKShop.DTOs;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminService : IAdminService
{
	private readonly DBContext _db;

	public AdminService(DBContext db)
	{
		_db = db;
	}

	public async Task<OverviewDTO> GetOverviewAsync(CancellationToken cancellationToken = default)
	{
		var endDate = DateTime.Today;
		var startCustomer = endDate.AddDays(-6);
		var startOrder = endDate.AddDays(-13);

		var customerRaw = await _db.Users
			.Where(u => u.Role == 0 && u.CreatedAt.Date >= startCustomer && u.CreatedAt.Date <= endDate)
			.GroupBy(u => u.CreatedAt.Date)
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
				MaHd = i.InvoiceId,
				HoTen = i.CustomerName ?? string.Empty,
				NgayDat = i.OrderDate,
				DiaChi = i.Address,
				CachThanhToan = i.PaymentMethod,
				CachVanChuyen = i.ShippingMethod,
				TrangThai = MapInvoiceStatus(i.StatusCode),
				GhiChu = i.Notes ?? string.Empty,
				DienThoai = i.PhoneNumber
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
			.OrderByDescending(p => p.ProductId)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(p => new HangHoaResponse
			{
				MaHh = p.ProductId,
				TenHH = p.ProductName,
				DonGia = p.Price ?? 0,
				Hinh = p.Image ?? string.Empty,
				MoTaNgan = p.Description ?? string.Empty,
				TenLoai = p.Category.CategoryName,
				GiamGia = p.Discount
			})
			.ToListAsync(cancellationToken);

		var categories = await _db.Categories
			.AsNoTracking()
			.OrderBy(c => c.CategoryName)
			.Select(c => new CategoryResponse
			{
				MaLoai = c.CategoryId,
				TenLoai = c.CategoryName,
				TenLoaiAlias = c.CategoryAlias,
				MoTa = c.Description,
				Hinh = c.Image
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
			.OrderBy(c => c.FullName)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(c => new ClientResponse
			{
				MaKH = c.CustomerId,
				HoTen = c.FullName,
				GioiTinh = c.Sex,
				NgaySinh = c.BirthDate,
				DiaChi = c.Address,
				DienThoai = c.PhoneNumber,
				Email = c.Email,
				VaiTro = c.User.Role,
				Hinh = c.Image
			})
			.ToListAsync(cancellationToken);

		var roles = await _db.Users
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
			.OrderBy(c => c.CategoryName)
			.Select(c => new CategoryResponse
			{
				MaLoai = c.CategoryId,
				TenLoai = c.CategoryName,
				TenLoaiAlias = c.CategoryAlias,
				MoTa = c.Description,
				Hinh = c.Image
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
