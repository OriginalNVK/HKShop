using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Services;

public class AdminCustomerService : IAdminCustomerService
{
	private readonly DBContext _db;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminCustomerService(DBContext db, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> CreateUserAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (await _db.Users.AnyAsync(u => u.Username == client.MaKH, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}

		var randomKey = Utils.GenerateRandomKey();
		var user = new User
		{
			Username = client.MaKH,
			CreatedAt = DateTime.Now,
			RandomKey = randomKey,
			Password = (client.MatKhau ?? "123456").ToMd5Hash(randomKey),
			Role = client.VaiTro,
			IsActive = true
		};

		await _db.Users.AddAsync(user, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);

		var customer = new Customer
		{
			CustomerId = client.MaKH,
			UserId = user.Id,
			FullName = client.HoTen,
			Sex = client.GioiTinh,
			BirthDate = client.NgaySinh,
			Address = client.DiaChi,
			PhoneNumber = client.DienThoai,
			Email = client.Email
		};

		if (image != null && image.Length > 0)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _db.Customers.AddAsync(customer, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Create user successfully");
	}

	public async Task<ClientResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var customer = await _db.Customers
			.AsNoTracking()
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.CustomerId == id, cancellationToken);

		if (customer == null)
		{
			return null;
		}

		return new ClientResponse
		{
			MaKH = customer.CustomerId,
			HoTen = customer.FullName,
			GioiTinh = customer.Sex,
			NgaySinh = customer.BirthDate,
			DiaChi = customer.Address,
			DienThoai = customer.PhoneNumber,
			Email = customer.Email,
			VaiTro = customer.User.Role,
			Hinh = customer.Image
		};
	}

	public async Task<ServiceResult> UpdateAsync(ClientRequest client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		var customer = await _db.Customers
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.CustomerId == client.MaKH, cancellationToken);

		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		customer.FullName = client.HoTen;
		customer.Sex = client.GioiTinh;
		customer.BirthDate = client.NgaySinh;
		customer.Address = client.DiaChi;
		customer.PhoneNumber = client.DienThoai;
		customer.Email = client.Email;

		if (image != null && image.Length > 0)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		customer.User.Role = client.VaiTro;
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Update user successfully");
	}

	public async Task<ServiceResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id, cancellationToken);
		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == customer.UserId, cancellationToken);
		_db.Customers.Remove(customer);
		if (user != null)
		{
			_db.Users.Remove(user);
		}

		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Delete user successfully");
	}
}
