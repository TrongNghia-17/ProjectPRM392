namespace DAL.Interfaces;

public interface ICartItemRepository
{
    Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId);
    Task AddAsync(CartItem cartItem);
    Task UpdateAsync(CartItem cartItem);
    Task DeleteAsync(Guid userId, Guid productId);
    Task DecreaseQuantityAsync(Guid userId, Guid productId, int decreaseAmount);
}
