namespace BLL.Interfaces;

public interface ICartItemService
{
    Task AddAsync(Guid userId, Guid productId, int quantity);
    Task DecreaseQuantityAsync(Guid userId, Guid productId, int decreaseAmount);
    Task<PagedCartItemResponse> GetCartItemsByUserIdAsync(Guid userId, int pageIndex, int pageSize);
    Task ClearCartAsync(Guid userId);
}
