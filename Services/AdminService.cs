using HKShop.DTOs;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class AdminService : IAdminService
{
	private readonly IUserRepository _userRepository;
	private readonly IInvoiceRepository _invoiceRepository;
	private readonly IProductRepository _productRepository;
	private readonly ICategoryRepository _categoryRepository;
	private readonly ICustomerRepository _customerRepository;

	public AdminService(
		IUserRepository userRepository,
		IInvoiceRepository invoiceRepository,
		IProductRepository productRepository,
		ICategoryRepository categoryRepository,
		ICustomerRepository customerRepository)
	{
		_userRepository = userRepository;
		_invoiceRepository = invoiceRepository;
		_productRepository = productRepository;
		_categoryRepository = categoryRepository;
		_customerRepository = customerRepository;
	}

	public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
	{
		var endDate = DateTime.Today;
		var startCustomer = endDate.AddDays(-6);
		var startOrder = endDate.AddDays(-13);

		var users = await _userRepository.GetAllAsync(cancellationToken);
		var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);

		var customerRaw = users
			.Where(u => u.Role == 0 && u.CreatedAt.Date >= startCustomer && u.CreatedAt.Date <= endDate)
			.GroupBy(u => u.CreatedAt.Date)
			.Select(g => new { Date = g.Key, Amount = g.Count() })
			.ToList();

		var orderRaw = invoices
			.Where(i => i.OrderDate.Date >= startOrder && i.OrderDate.Date <= endDate)
			.GroupBy(i => i.OrderDate.Date)
			.Select(g => new { Date = g.Key, Amount = g.Count() })
			.ToList();

		var customerMap = customerRaw.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Amount);
		var orderMap = orderRaw.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Amount);

		var result = new DashboardOverviewDto();
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

	public async Task<List<InvoiceDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
	{
		var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);

		return invoices
			.OrderByDescending(i => i.OrderDate)
			.Select(i => new InvoiceDto
			{
				InvoiceId = i.InvoiceId,
				CustomerName = i.CustomerName ?? string.Empty,
				OrderDate = i.OrderDate,
				Address = i.Address,
				PaymentMethod = i.PaymentMethod,
				ShippingMethod = i.ShippingMethod,
				Status = MapInvoiceStatus(i.StatusCode),
				Notes = i.Notes ?? string.Empty,
				PhoneNumber = i.PhoneNumber
			})
			.ToList();
	}

	public async Task<AdminProductsPageResult> GetProductsAsync(int pageNumber, int pageSize, int? categoryId, CancellationToken cancellationToken = default)
	{
		pageNumber = Math.Max(1, pageNumber);
		pageSize = Math.Max(1, pageSize);

		var allProducts = await _productRepository.GetAllAsync(cancellationToken);
		var query = allProducts.AsQueryable();
		if (categoryId.HasValue && categoryId.Value != 0)
		{
			query = query.Where(p => p.CategoryId == categoryId.Value);
		}

		var totalCount = query.Count();
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var products = query
			.OrderByDescending(p => p.ProductId)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(p => new ProductResponseDto
			{
				ProductId = p.ProductId,
				ProductName = p.ProductName,
				AliasName = p.AliasName,
				CategoryId = p.CategoryId,
				UnitDescription = p.Description,
				Price = p.Price,
				ImageUrl = p.Image,
				ManufactureDate = p.CreatedAt,
				Discount = p.Discount,
				Views = p.Views,
				Description = p.Description,
				Category = p.Category
			})
			.ToList();

		var categories = (await _categoryRepository.GetAllAsync(cancellationToken))
			.OrderBy(c => c.CategoryName)
			.Select(c => new CategoryDto
			{
				CategoryId = c.CategoryId,
				CategoryName = c.CategoryName,
				CategoryAlias = c.CategoryAlias,
				Description = c.Description,
				ImageUrl = c.Image
			})
			.ToList();

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

		var query = (await _customerRepository.GetAllAsync(cancellationToken)).AsQueryable();
		if (role.HasValue)
		{
			query = query.Where(c => c.User.Role == role.Value);
		}

		var totalCount = query.Count();
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var clients = query
			.OrderBy(c => c.FullName)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(c => new CustomerResponseDto
			{
				CustomerId = c.CustomerId,
				FullName = c.FullName,
				Gender = c.Sex,
				BirthDate = c.BirthDate,
				Address = c.Address,
				PhoneNumber = c.PhoneNumber,
				Email = c.Email,
				Role = c.User.Role,
				ImageUrl = c.Image
			})
			.ToList();

		var roles = (await _userRepository.GetAllAsync(cancellationToken))
			.Select(u => u.Role)
			.Distinct()
			.OrderBy(x => x)
			.ToList();

		return new AdminClientsPageResult
		{
			Clients = clients,
			Roles = roles,
			TotalCount = totalCount,
			TotalPages = totalPages == 0 ? 1 : totalPages
		};
	}

	public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		return (await _categoryRepository.GetAllAsync(cancellationToken))
			.OrderBy(c => c.CategoryName)
			.Select(c => new CategoryDto
			{
				CategoryId = c.CategoryId,
				CategoryName = c.CategoryName,
				CategoryAlias = c.CategoryAlias,
				Description = c.Description,
				ImageUrl = c.Image
			})
			.ToList();
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
