namespace DAL.Implements;

public class CartItemRepository(ElectronicStoreDbContext context) : ICartItemRepository
{
    private readonly ElectronicStoreDbContext _context = context;

    public async Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
    }

    public async Task AddAsync(CartItem cartItem)
    {
        if (cartItem == null)
            throw new ArgumentNullException(nameof(cartItem));

        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CartItem cartItem)
    {
        if (cartItem == null)
            throw new ArgumentNullException(nameof(cartItem));

        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId, Guid productId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        if (cartItem == null)
            throw new KeyNotFoundException($"Cart item for user {userId} and product {productId} not found.");

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task DecreaseQuantityAsync(Guid userId, Guid productId, int decreaseAmount)
    {
        if (decreaseAmount <= 0)
            throw new ArgumentException("Decrease amount must be greater than zero.", nameof(decreaseAmount));

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        if (cartItem == null)
            throw new KeyNotFoundException($"Cart item for user {userId} and product {productId} not found.");
        if (cartItem.Quantity < decreaseAmount)
            throw new InvalidOperationException($"Cannot decrease {decreaseAmount} items. Current quantity is {cartItem.Quantity}.");

        cartItem.Quantity -= decreaseAmount;

        if (cartItem.Quantity == 0)
        {
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            _context.CartItems.Update(cartItem);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<CartItem> CartItems, int TotalCount)> GetByUserIdAsync(Guid userId, int pageIndex, int pageSize)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (pageIndex < 0)
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        var query = _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Product); 

        var totalCount = await query.CountAsync();
        var cartItems = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (cartItems, totalCount);
    }
}
