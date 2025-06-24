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

        //if (cartItem.AddedAt.Kind != DateTimeKind.Utc)
        //{
        //    cartItem.AddedAt = DateTime.SpecifyKind(cartItem.AddedAt, DateTimeKind.Utc);
        //}

        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CartItem cartItem)
    {
        if (cartItem == null)
            throw new ArgumentNullException(nameof(cartItem));

        //if (cartItem.AddedAt.Kind != DateTimeKind.Utc)
        //{
        //    cartItem.AddedAt = DateTime.SpecifyKind(cartItem.AddedAt, DateTimeKind.Utc);
        //}

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
        //if (cartItem.AddedAt.Kind != DateTimeKind.Utc)
        //{
        //    cartItem.AddedAt = DateTime.SpecifyKind(cartItem.AddedAt, DateTimeKind.Utc);
        //}

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
}
