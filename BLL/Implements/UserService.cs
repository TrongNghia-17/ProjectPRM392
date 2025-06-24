namespace BLL.Implements;

public class UserService(IMemoryCache memoryCache, IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private const string CacheKey = "AllUsers_{0}_{1}";

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

}
