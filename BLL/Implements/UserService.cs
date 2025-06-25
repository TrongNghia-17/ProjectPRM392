namespace BLL.Implements;

public class UserService
    (IMemoryCache memoryCache,
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;
    private const string CacheKey = "AllUsers_{0}_{1}";
    private const string UserCacheKey = "User_{0}";

    public async Task<PagedUserResponse> GetAllUsersAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        var cacheKey = string.Format(CacheKey, pageIndex, pageSize);
        var cachedResponse = _memoryCache.Get<PagedUserResponse>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        var (users, totalCount) = await _userRepository.GetAllAsync(pageIndex, pageSize);
        var userResponses = _mapper.Map<IEnumerable<UserResponse>>(users);

        var response = new PagedUserResponse
        {
            Users = userResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(10));

        return response;
    }

    public async Task<AuthResponse> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");
        return new AuthResponse { UserId = user.UserId, Email = user.Email, Token = string.Empty };
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
}
