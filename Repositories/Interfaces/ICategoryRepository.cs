using HKShop.Domain;

namespace HKShop.Repositories.Interfaces;

public interface ICategoryRepository
{
    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of categories</returns>
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a category by its unique ID.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Category or null if not found</returns>
    Task<Category?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created category</returns>
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a category by its unique ID.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count the number of products in a category.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Number of products in the category</returns>
    Task<int> CountProductsAsync(int categoryId, CancellationToken cancellationToken = default);
}

