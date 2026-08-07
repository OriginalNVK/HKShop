using HKShop.Domain;

namespace HKShop.Repositories.Interfaces;

public interface IEmployeeRepository
{
    /// <summary>
    /// Get all information about employees, including their user details.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of admins</returns>
	Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get employee details by their unique employee ID, including user and invoice information.
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Employee details or null if not found</returns>
	Task<Employee?> GetByIdAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get employee details by their associated user ID, including user information.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Employee details or null if not found</returns>
	Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new employee record in the database. The provided Employee object should contain all necessary information, 
    /// including the associated UserId. Returns the created Employee with its generated EmployeeId. If the creation fails, 
    /// an exception will be thrown.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created employee</returns>
	Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing employee's information. 
    /// The provided Employee object must have a valid EmployeeId that exists in the database.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
	Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an employee record from the database by its unique employee ID. 
    /// This will remove the employee's information, but not the associated user account.
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
	Task<bool> DeleteAsync(int employeeId, CancellationToken cancellationToken = default);
}