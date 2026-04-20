using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface IDetailInvoiceRepository
{
    /// <summary>
    /// Get all detail invoices for a specific invoice ID. 
    /// This method retrieves a list of DetailInvoice objects that are associated with the given invoice ID.
    /// If no detail invoices are found for the specified invoice ID, an empty list will be returned.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of detail invoices</returns>
    Task<List<DetailInvoice>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a specific detail invoice by its unique identifier.
    /// </summary>
    /// <param name="detailInvoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detail invoice or null if not found</returns>
    Task<DetailInvoice?> GetByIdAsync(int detailInvoiceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new detail invoice in the database. This method adds a new DetailInvoice record
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created detail invoice</returns>
    Task<DetailInvoice> CreateAsync(DetailInvoice detail, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing detail invoice's information.
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the detail invoice was updated successfully, false otherwise</returns>
    Task<bool> UpdateAsync(DetailInvoice detail, CancellationToken cancellationToken = default);
    /// <summary>
    /// Delete a detail invoice from the database.
    /// </summary>
    /// <param name="detailInvoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the detail invoice was deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int detailInvoiceId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get the subtotal for a specific invoice.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Invoice subtotal</returns>
    Task<decimal> GetInvoiceSubTotalAsync(int invoiceId, CancellationToken cancellationToken = default);
}