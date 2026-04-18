using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface IProductRepository
{
    /// <summary>
    /// Get all products with optional filtering by category and keyword search.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of products</returns>
	Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Get a paginated list of products with optional filtering by category and keyword search.
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="categoryId"></param>
    /// <param name="keyword"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Page of products</returns>
    Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize, int? categoryId = null, string? keyword = null, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Get a specific product by its unique identifier. 
    /// This method retrieves detailed information about a single product, 
    /// including its associated category information. 
    /// It is designed to be used in scenarios where you need to display or process the details of a specific product,
    /// such as on a product detail page or when editing a product in an admin dashboard.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Product or null if not found</returns>
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Create a new product in the database. This method adds a new product record
    /// </summary>
    /// <param name="product"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created product</returns>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Update an existing product's information. This method allows you to modify the details of an existing product,
    /// such as its name, description, price, stock quantity, and category.
    /// </summary>
    /// <param name="product"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the product was updated, false otherwise</returns>
    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
	
    /// <summary>
    /// Delete a product from the database. 
    /// This method removes a product record based on its unique identifier.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the product was deleted, false otherwise</returns>
    Task<bool> DeleteAsync(int productId, CancellationToken cancellationToken = default);
}