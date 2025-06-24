namespace BLL.Implements;

public class CartItemService
    (IProductRepository productRepository, 
    ICartItemRepository cartItemRepository,
    IMapper mapper) : ICartItemService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
    private readonly IMapper _mapper = mapper;

    public async Task AddAsync(Guid userId, Guid productId, int quantity)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        // Kiểm tra sản phẩm tồn tại
        var product = await _productRepository.GetByIdAsync(productId) ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

        // Kiểm tra tồn kho
        int availableStock = await _productRepository.GetQuantityByIdAsync(productId);
        if (availableStock < quantity)
            throw new InvalidOperationException($"Only {availableStock} items available in stock.");

        // Kiểm tra giỏ hàng hiện tại
        var existingCartItem = await _cartItemRepository.GetByUserAndProductAsync(userId, productId);
        if (existingCartItem != null)
        {
            // Cập nhật số lượng nếu sản phẩm đã trong giỏ
            int newQuantity = existingCartItem.Quantity + quantity;
            if (newQuantity > availableStock)
                throw new InvalidOperationException($"Cannot add {quantity} items. Only {availableStock - existingCartItem.Quantity} items available.");

            existingCartItem.Quantity = newQuantity;
            await _cartItemRepository.UpdateAsync(existingCartItem);
        }
        else
        {
            // Thêm mới vào giỏ hàng
            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
            await _cartItemRepository.AddAsync(cartItem);
        }

        // Giảm tồn kho (nếu cần áp dụng ngay)
        //product.Stock -= quantity;
        //await _productRepository.UpdateAsync(product);
    }

    public async Task DecreaseQuantityAsync(Guid userId, Guid productId, int quantity)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentException("Decrease amount must be greater than zero.", nameof(quantity));

        // Kiểm tra sản phẩm tồn tại
        var product = await _productRepository.GetByIdAsync(productId) ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

        // Giảm số lượng trong giỏ hàng
        var existingCartItem = await _cartItemRepository.GetByUserAndProductAsync(userId, productId);
        if (existingCartItem == null)
            throw new KeyNotFoundException($"Product {productId} is not in the cart for user {userId}.");
        if (existingCartItem.Quantity < quantity)
            throw new InvalidOperationException($"Cannot decrease {quantity} items. Current quantity is {existingCartItem.Quantity}.");

        await _cartItemRepository.DecreaseQuantityAsync(userId, productId, quantity);

        // Tăng tồn kho tương ứng với số lượng giảm
        //product.Quantity += quantity;

        //if (product.CreatedAt.Kind != DateTimeKind.Utc)
        //{
        //    product.CreatedAt = DateTime.SpecifyKind(product.CreatedAt, DateTimeKind.Utc);
        //}

        //await _productRepository.UpdateAsync(product);
    }

    public async Task<PagedCartItemResponse> GetCartItemsByUserIdAsync(Guid userId, int pageIndex, int pageSize)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (pageIndex < 0)
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        var (cartItems, totalCount) = await _cartItemRepository.GetByUserIdAsync(userId, pageIndex, pageSize);
        var cartItemResponses = _mapper.Map<IEnumerable<CartItemResponse>>(cartItems);

        return new PagedCartItemResponse
        {
            CartItems = cartItemResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}
