using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Domain;
using HKShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HKShop.Services;

public class CustomerService : ICustomerService
{
	private readonly HKShopDbContext _db;
	private readonly IGenerateToken _generateToken;
	private readonly ICloudinaryService _cloudinaryService;

	public CustomerService(HKShopDbContext db, IGenerateToken generateToken, ICloudinaryService cloudinaryService)
	{
		_db = db;
		_generateToken = generateToken;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> RegisterAsync(DangKyRequest model, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (await _db.AppUsers.AnyAsync(u => u.Username == model.TenDangNhap, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}

		var user = new AppUser
		{
			Username = model.TenDangNhap,
			CreatedDate = DateTime.Now,
			RandomKey = Utils.GenerateRandomKey(),
			Password = (model.MatKhau ?? string.Empty).ToMd5Hash(Utils.GenerateRandomKey()),
			IsActive = true,
			Role = 0
		};
		user.Password = (model.MatKhau ?? string.Empty).ToMd5Hash(user.RandomKey);

		await _db.AppUsers.AddAsync(user, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);

		var customer = new Customer
		{
			UserId = user.Id,
			Fullname = model.HoTen,
			Gender = model.GioiTinh,
			Birthday = model.NgaySinh ?? DateOnly.MinValue,
			Address = model.DiaChi,
			Phone = model.DienThoai,
			Email = model.Email
		};

		if (image != null)
		{
			customer.Avatar = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _db.Customers.AddAsync(customer, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return ServiceResult.Ok("Register success");
	}

	public async Task<LoginResult> LoginAsync(DangNhapRequest model, string? returnUrl, CancellationToken cancellationToken = default)
	{
		var user = await _db.AppUsers.SingleOrDefaultAsync(u => u.Username == model.Username, cancellationToken);
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

		var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
		if (customer == null)
		{
			return new LoginResult { Success = false, Message = "User profile not found" };
		}

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, customer.Email),
			new Claim(ClaimTypes.Name, customer.Fullname),
			new Claim(Constants.CLAIM_CUSTOMERID, user.Role == 0 ? customer.Id.ToString() : user.Id.ToString()),
			new Claim(ClaimTypes.Role, user.Role.ToString()),
			new Claim("Avatar", customer.Avatar ?? string.Empty)
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
