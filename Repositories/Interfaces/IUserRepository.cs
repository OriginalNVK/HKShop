using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Get all users in the system. This method retrieves a list of 
    /// all user records from the database,
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of users</returns>
	public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a user by their unique identifier. 
    /// This method retrieves a single user record based on the provided user ID
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>User or null if not found</returns>
	public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a user by their username. 
    /// This method retrieves a user record based on the provided username,
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>User or null if not found</returns>
	public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a username already exists in the system. 
    /// This method is used to validate the uniqueness of a username before creating or updating a user record.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the username exists, false otherwise</returns>
	public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new user in the database. This method adds a new user record to the database,
    /// and returns the created user with its assigned ID.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created user</returns>
	public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing user's information. 
    /// This method modifies the details of an existing user record in the database.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the update was successful, false otherwise</returns>
	public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a user from the database. 
    /// This method removes a user record based on their unique identifier (user ID).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the deletion was successful, false otherwise</returns>
	public Task<bool> DeleteAsync(int userId, CancellationToken cancellationToken = default);
}