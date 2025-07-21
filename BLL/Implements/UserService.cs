using BLL.DTOs.OdersDTO;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implements;

public class UserService
    (IMemoryCache memoryCache,
    IUserRepository userRepository,
    ICartItemService cartItemService,
    IOrderService orderService,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICartItemService _cartItemService = cartItemService;
    private readonly IOrderService _orderService = orderService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;
    private const string CacheKey = "AllUsers_{0}_{1}";
    private const string UserCacheKey = "User_{0}";

    public async Task<List<UserResponse>> GetAllUsersAsync()
    {       
        //var cacheKey = string.Format(CacheKey, pageIndex, pageSize);
        //var cachedResponse = _memoryCache.Get<PagedUserResponse>(cacheKey);

        //if (cachedResponse != null)
        //{
        //    return cachedResponse;
        //}

        var users = await _userRepository.GetAllAsync();
        var userResponses = _mapper.Map<List<UserResponse>>(users);

        //_memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(10));

        return userResponses;
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid id)
    {
        var cacheKey = string.Format(UserCacheKey, id);
        if (_memoryCache.TryGetValue(cacheKey, out UserResponse? cachedUser) && cachedUser != null)
        {
            _logger.LogInformation("Retrieved user with ID {UserId} from cache.", id);
            return cachedUser;
        }

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");
        var response = _mapper.Map<UserResponse>(user);

        _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Cached user with ID {UserId}.", id);

        return response;
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        // Ánh xạ dữ liệu từ request sang entity
        _mapper.Map(request, user);

        await _userRepository.UpdateAsync(user);

        // Xóa cache liên quan
        _memoryCache.Remove(string.Format(UserCacheKey, id));
    }

    public async Task DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Attempting to delete user with ID {UserId}", id);

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepository.DeleteAsync(user);

        // Xóa cache liên quan
        _memoryCache.Remove(string.Format(UserCacheKey, id));
        _logger.LogInformation("Deleted user with ID {UserId} and cleared related cache.", id);
    }

    public async Task SelfUpdateUserAsync(Guid userId, SelfUpdateUserRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("Self-update request is null for user ID {UserId}.", userId);
            throw new ArgumentNullException(nameof(request));
        }

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        // Kiểm tra email trùng lặp (nếu email thay đổi)
        if (user.Email != request.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null && existingUser.UserId != userId)
            {
                _logger.LogWarning("Email {Email} already exists for another user.", request.Email);
                throw new InvalidOperationException("Email is already in use.");
            }
        }

        // Ánh xạ dữ liệu từ request sang entity
        _mapper.Map(request, user);

        await _userRepository.UpdateAsync(user);

        // Xóa cache liên quan
        _memoryCache.Remove(string.Format(UserCacheKey, userId));
        _logger.LogInformation("User with ID {UserId} updated their information successfully.", userId);
    }

    public async Task<OrderResponseDto> UpdateUserAndCreateOrderAsync(Guid userId)
    {
        //using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Cập nhật thông tin người dùng
            //var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            //user.FullName = request.FullName ?? user.FullName;
            ////user.Email = request.Email ?? user.Email;
            //user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            //user.Address = request.Address ?? user.Address;

            //await _userRepository.UpdateAsync(user);

            var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

            // Lấy giỏ hàng của người dùng
            var cartItems = (await _cartItemService.GetCartItemsByUserIdAsync(userId, 0, int.MaxValue)).CartItems;
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty. Cannot create an order.");

            // Tạo yêu cầu tạo đơn hàng
            var createOrderRequest = new CreateOrderRequest
            {
                UserId = userId,
                ShippingAddress = user.Address,
                Items = cartItems.Select(ci => new OrderItemDto // Changed CreateOrderItemRequest to OrderItemDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    // Assuming you have Price and OrderItemId available in CartItemDto or need to fetch them
                    // For now, setting them to default values or you might need to adjust this based on your CartItemDto structure
                    Price = ci.Price, // Assuming Price exists in CartItemDto
                    OrderItemId = Guid.NewGuid() // Generate a new GUID or get from CartItemDto if it exists
                }).ToList()
            };

            // Tạo đơn hàng
            var createdOrder = await _orderService.CreateOrderAsync(createOrderRequest);

            // Xóa giỏ hàng
            await _cartItemService.ClearCartAsync(userId);

            // Commit transaction
            //await transaction.CommitAsync();
            return createdOrder;
        }
        catch
        {
            //await transaction.RollbackAsync();
            throw;
        }
    }
}
