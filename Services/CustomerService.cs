using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HKShop.Services;

public class CustomerService : ICustomerService
{
	private readonly IUserRepository _user;
	private readonly ICustomerRepository _customer;
	private readonly IGenerateToken _generateToken;
	private readonly ICloudinaryService _cloudinaryService;

	public CustomerService(IUserRepository user, ICustomerRepository customer, IGenerateToken generateToken, ICloudinaryService cloudinaryService)
	{
		_user = user;
		_customer = customer;
		_generateToken = generateToken;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> RegisterAsync(RegisterRequestDto model, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if(await _user.UsernameExistsAsync(model.Username, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}
		if (await _customer.GetByUsernameAsync(model.Username, cancellationToken) != null)
		{
			return ServiceResult.Fail("Username already exists");
		}

		var user = new User
		{
			Username = model.Username,
			CreatedAt = DateTime.Now,
			RandomKey = Utils.GenerateRandomKey(),
			IsActive = true,
			Role = 0
		};
		user.Password = (model.Password ?? string.Empty).ToMd5Hash(user.RandomKey);
		await _user.CreateAsync(user, cancellationToken);

		var customer = new Customer
		{
			CustomerId = model.Username,
			UserId = user.Id,
			FullName = model.FullName,
			Sex = model.Gender,
			BirthDate = model.BirthDate ?? DateOnly.MinValue,
			Address = model.Address,
			PhoneNumber = model.PhoneNumber,
			Email = model.Email
		};

		if (image != null)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _customer.CreateAsync(customer, cancellationToken);
		return ServiceResult.Ok("Register success");
	}

	public async Task<LoginResult> LoginAsync(LoginRequestDto model, string? returnUrl, CancellationToken cancellationToken = default)
	{
		var user = await _user.GetByUsernameAsync(model.Username, cancellationToken);
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

		var customer = await _customer.GetByUsernameAsync(user.Username, cancellationToken);
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
