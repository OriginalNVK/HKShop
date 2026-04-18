using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface IAdminRepository
{
    /// <summary>
    /// Get all information about admins, including their user details.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of admins</returns>
	Task<List<Admin>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get admin details by their unique admin ID, including user and invoice information.
    /// </summary>
    /// <param name="adminId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Admin details or null if not found</returns>
	Task<Admin?> GetByIdAsync(string adminId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get admin details by their associated user ID, including user information.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Admin details or null if not found</returns>
	Task<Admin?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new admin record in the database. The provided Admin object should contain all necessary information, 
    /// including the associated UserId. Returns the created Admin with its generated AdminId. If the creation fails, 
    /// an exception will be thrown.
    /// </summary>
    /// <param name="admin"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created admin</returns>
	Task<Admin> CreateAsync(Admin admin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing admin's information. 
    /// The provided Admin object must have a valid AdminId that exists in the database.
    /// </summary>
    /// <param name="admin"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
	Task<bool> UpdateAsync(Admin admin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an admin record from the database by its unique admin ID. 
    /// This will remove the admin's information, but not the associated user account.
    /// </summary>
    /// <param name="adminId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
	Task<bool> DeleteAsync(string adminId, CancellationToken cancellationToken = default);
}