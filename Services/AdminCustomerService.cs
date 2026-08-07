using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminCustomerService : IAdminCustomerService
{
	private readonly HKShopDbContext _db;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminCustomerService(HKShopDbContext db, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> CreateUserAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (await _db.AppUsers.AnyAsync(u => u.Username == client.MaKH, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}

		var randomKey = Utils.GenerateRandomKey();
		var user = new AppUser
		{
			Username = client.MaKH,
			CreatedDate = DateTime.Now,
			RandomKey = randomKey,
			Password = (client.MatKhau ?? "123456").ToMd5Hash(randomKey),
			Role = client.VaiTro,
			IsActive = true
		};

		await _db.AppUsers.AddAsync(user, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);

		var customer = new Customer
		{
			UserId = user.Id,
			Fullname = client.HoTen,
			Gender = client.GioiTinh,
			Birthday = client.NgaySinh,
			Address = client.DiaChi,
			Phone = client.DienThoai,
			Email = client.Email
		};

		if (image != null && image.Length > 0)
		{
			customer.Avatar = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _db.Customers.AddAsync(customer, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Create user successfully");
	}

	public async Task<ClientResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		if (!int.TryParse(id, out var customerId))
		{
			return null;
		}

		var customer = await _db.Customers
			.AsNoTracking()
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

		if (customer == null)
		{
			return null;
		}

		return new ClientResponse
		{
			CustomerId = customer.Id.ToString(),
			FullName = customer.Fullname,
			Gender = customer.Gender,
			BirthDate = customer.Birthday,
			Address = customer.Address,
			PhoneNumber = customer.Phone,
			Email = customer.Email,
			Role = customer.User.Role,
			ImageUrl = customer.Avatar
		};
	}

	public async Task<ServiceResult> UpdateAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (!int.TryParse(client.MaKH, out var customerId))
		{
			return ServiceResult.Fail("User not found");
		}

		var customer = await _db.Customers
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		customer.Fullname = client.HoTen;
		customer.Gender = client.GioiTinh;
		customer.Birthday = client.NgaySinh;
		customer.Address = client.DiaChi;
		customer.Phone = client.DienThoai;
		customer.Email = client.Email;

		if (image != null && image.Length > 0)
		{
			customer.Avatar = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		customer.User.Role = client.VaiTro;
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Update user successfully");
	}

	public async Task<ServiceResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		if (!int.TryParse(id, out var customerId))
		{
			return ServiceResult.Fail("User not found");
		}

		var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Id == customer.UserId, cancellationToken);
		_db.Customers.Remove(customer);
		if (user != null)
		{
			_db.AppUsers.Remove(user);
		}

		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Delete user successfully");
	}
}
