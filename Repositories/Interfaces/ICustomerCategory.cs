using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface ICustomerRepository
{
    /// <summary>
    /// Get all customers from the database. This method retrieves a list of all customers, 
    /// including their associated user information and invoices. 
    /// It is designed to be used in scenarios where you need to display or process a 
    /// complete list of customers, such as in an admin dashboard or customer management system.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Get list of customers</returns>
	public Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific customer by their unique username. 
    /// This method retrieves detailed information about a single customer,
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Customer or null if not found</returns>
	public Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new customer in the database. This method adds a new customer record,
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created customer</returns>
	public Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing customer's information. 
    /// This method allows you to modify the details of an existing customer,
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the customer was updated successfully, false otherwise</returns>
	public Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a customer from the database. 
    /// This method removes a customer record based on their unique identifier.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the customer was deleted successfully, false otherwise</returns>
	public Task<bool> DeleteAsync(string customerId, CancellationToken cancellationToken = default);
}