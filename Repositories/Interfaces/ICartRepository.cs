using HKShop.Models;

namespace HKShop.Repositories.Interfaces;

public interface ICartRepository
{
    /// <summary>
    /// Get all cart items for a specific customer. 
    /// This method retrieves the current contents of the customer's shopping cart, 
    /// including product details and quantities.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of cart items</returns>
    public Task<List<Cart>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific cart item for a customer by product ID.
    /// This method checks if a particular product is already in the customer's cart and retrieves its details
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Get the cart item or null if not found</returns>
	public Task<Cart?> GetItemAsync(string customerId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new item to the cart or update the quantity if the item already exists.
    /// If the specified quantity is zero or negative, the item will not be added or updated.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated or added cart item, or null if unsuccessful</returns>
	public Task<Cart?> AddOrUpdateItemAsync(string customerId, int productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the quantity of an existing cart item. If the specified quantity is zero or negative, 
    /// the item will be removed from the cart.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the quantity was updated successfully, false otherwise</returns>
	public Task<bool> UpdateQuantityAsync(string customerId, int productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove an item from the cart based on the customer ID and product ID.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if the item was removed successfully, false otherwise</returns>
	public Task<bool> RemoveItemAsync(string customerId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all items from a customer's cart. 
    /// This will remove all products and reset the cart to an empty state.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of items removed</returns>
	public Task<int> ClearCartAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate the total cost of all items in a customer's cart. 
    /// This method sums up the price of each product multiplied by its quantity to 
    /// return the total amount that the customer would need to pay for their current cart contents.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Get the total cost of the cart</returns>
	public Task<decimal> GetCartTotalAsync(string customerId, CancellationToken cancellationToken = default);
}