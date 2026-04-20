using HKShop.DTOs;
using HKShop.Helpers;
using HKShop.Models;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class AdminCustomerService : IAdminCustomerService
{
	private readonly IUserRepository _userRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly ICloudinaryService _cloudinaryService;

	public AdminCustomerService(IUserRepository userRepository, ICustomerRepository customerRepository, ICloudinaryService cloudinaryService)
	{
		_userRepository = userRepository;
		_customerRepository = customerRepository;
		_cloudinaryService = cloudinaryService;
	}

	public async Task<ServiceResult> CreateUserAsync(CustomerRequestDto client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		if (await _userRepository.UsernameExistsAsync(client.CustomerId, cancellationToken))
		{
			return ServiceResult.Fail("Username already exists");
		}

		var randomKey = Utils.GenerateRandomKey();
		var user = new User
		{
			Username = client.CustomerId,
			CreatedAt = DateTime.Now,
			RandomKey = randomKey,
			Password = (client.Password ?? "123456").ToMd5Hash(randomKey),
			Role = client.Role,
			IsActive = true
		};

		await _userRepository.CreateAsync(user, cancellationToken);

		var customer = new Customer
		{
			CustomerId = client.CustomerId,
			UserId = user.Id,
			FullName = client.FullName,
			Sex = client.Gender,
			BirthDate = client.BirthDate,
			Address = client.Address,
			PhoneNumber = client.PhoneNumber,
			Email = client.Email
		};

		if (image != null && image.Length > 0)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		await _customerRepository.CreateAsync(customer, cancellationToken);
		return ServiceResult.Ok("Create user successfully");
	}

	public async Task<CustomerResponseDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var customer = await _customerRepository.GetByUsernameAsync(id, cancellationToken);

		if (customer == null)
		{
			return null;
		}

		return new CustomerResponseDto
		{
			CustomerId = customer.CustomerId,
			FullName = customer.FullName,
			Gender = customer.Sex,
			BirthDate = customer.BirthDate,
			Address = customer.Address,
			PhoneNumber = customer.PhoneNumber,
			Email = customer.Email,
			Role = customer.User.Role,
			ImageUrl = customer.Image
		};
	}

	public async Task<ServiceResult> UpdateAsync(CustomerRequestDto client, IFormFile? image, CancellationToken cancellationToken = default)
	{
		var customer = await _customerRepository.GetByUsernameAsync(client.CustomerId, cancellationToken);

		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		customer.FullName = client.FullName;
		customer.Sex = client.Gender;
		customer.BirthDate = client.BirthDate;
		customer.Address = client.Address;
		customer.PhoneNumber = client.PhoneNumber;
		customer.Email = client.Email;

		if (image != null && image.Length > 0)
		{
			customer.Image = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
		}

		customer.User.Role = client.Role;
		var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);
		var updatedUser = await _userRepository.UpdateAsync(customer.User, cancellationToken);
		if (!updatedCustomer || !updatedUser)
		{
			return ServiceResult.Fail("Update failed");
		}

		return ServiceResult.Ok("Update user successfully");
	}

	public async Task<ServiceResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		var customer = await _customerRepository.GetByUsernameAsync(id, cancellationToken);
		if (customer == null)
		{
			return ServiceResult.Fail("User not found");
		}

		await _customerRepository.DeleteAsync(id, cancellationToken);
		await _userRepository.DeleteAsync(customer.UserId, cancellationToken);
		return ServiceResult.Ok("Delete user successfully");
	}
}
