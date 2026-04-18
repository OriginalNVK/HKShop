using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HKShop.Services;

public class CustomerService : ICustomerService
{
	private readonly DBContext _db;
	private readonly IGenerateToken _generateToken;
	private readonly ICloudinaryService _cloudinaryService;

	public CustomerService(DBContext db, IGenerateToken generateToken, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_generateToken = generateToken;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> RegisterAsync(DangKyRequest model, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (await _db.Users.AnyAsync(u => u.Username == model.TenDangNhap, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}

		var user = new User
		{
			Username = model.TenDangNhap,
			CreatedAt = DateTime.Now,
			RandomKey = Utils.GenerateRandomKey(),
			Password = (model.MatKhau ?? string.Empty).ToMd5Hash(Utils.GenerateRandomKey()),
			IsActive = true,
			Role = 0
		};
		user.Password = (model.MatKhau ?? string.Empty).ToMd5Hash(user.RandomKey);

		await _db.Users.AddAsync(user, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);

		var customer = new Customer
		{
			CustomerId = model.TenDangNhap,
			UserId = user.Id,
			FullName = model.HoTen,
			Sex = model.GioiTinh,
			BirthDate = model.NgaySinh ?? DateOnly.MinValue,
			Address = model.DiaChi,
			PhoneNumber = model.DienThoai,
			Email = model.Email
		};

		if (image != null)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _db.Customers.AddAsync(customer, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Register success");
	}

	public async Task<LoginResult> LoginAsync(DangNhapRequest model, string? returnUrl, CancellationToken cancellationToken = default)
	{
		var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == model.Username, cancellationToken);
		if (user == null)
		{
			return new LoginResult { Success = false, Message = "Invalid username or password" };
		}

		if (!user.IsActive)
		{
			return new LoginResult { Success = false, Message = "Your account has been locked" };
		}

		if (user.Password != model.Password.ToMd5Hash(user.RandomKey))
		{
			return new LoginResult { Success = false, Message = "Invalid username or password" };
		}

		var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == user.Username, cancellationToken);
		if (customer == null)
		{
			return new LoginResult { Success = false, Message = "User profile not found" };
		}

		var claims = new List<Claim>
		{
			new(ClaimTypes.Email, customer.Email),
			new(ClaimTypes.Name, customer.FullName),
			new(Constants.CLAIM_CUSTOMERID, customer.CustomerId),
			new(ClaimTypes.Role, user.Role.ToString()),
			new("Avatar", customer.Image ?? string.Empty)
		};

		var redirectUrl = !string.IsNullOrWhiteSpace(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
			? returnUrl
			: user.Role switch
			{
				1 => "/admin",
				_ => "/"
			};

		return new LoginResult
		{
			Success = true,
			Message = "Login success",
			Token = _generateToken.GenerateJwtToken(claims),
			RedirectUrl = redirectUrl
		};
	}
}
