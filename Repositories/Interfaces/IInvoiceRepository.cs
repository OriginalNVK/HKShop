using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface IInvoiceRepository
{
    /// <summary>
    /// Get all invoices, ordered by order date descending. 
    /// Includes related customer and admin data.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of invoices</returns>
	Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Get an invoice by its unique identifier. 
    /// Includes related customer, admin, and detail invoice data.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Invoice if found, otherwise null</returns>
    Task<Invoice?> GetByIdAsync(int invoiceId, CancellationToken cancellationToken = default);
	
    /// <summary>
	/// Get invoices by customer ID, ordered by order date descending.
	/// Includes related customer and admin data.
	/// </summary>
	/// <param name="customerId"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>List of invoices</returns>
	Task<List<Invoice>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Create a new invoice in the database. 
    /// This method adds a new invoice record, 
    /// along with its associated detail invoices, to the database.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created invoice</returns>
    Task<Invoice> CreateAsync(Invoice invoice, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Update the status of an existing invoice. 
    /// This method allows you to change the status code of an invoice,
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="statusCode"></param>
    /// <param name="deliveryDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the update was successful, otherwise false</returns>
    Task<bool> UpdateStatusAsync(int invoiceId, int statusCode, DateOnly? deliveryDate = null, CancellationToken cancellationToken = default);
	
    /// <summary>
	/// Assign an admin to an existing invoice.
	/// </summary>
	/// <param name="invoiceId"></param>
	/// <param name="adminId"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>True if the assignment was successful, otherwise false</returns>
	Task<bool> AssignAdminAsync(int invoiceId, string adminId, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Delete an invoice from the database. 
    /// This method removes an invoice record based on its unique identifier.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the deletion was successful, otherwise false</returns>
    Task<bool> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default);
}