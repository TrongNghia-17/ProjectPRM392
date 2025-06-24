namespace BLL.Interfaces;

public interface IUserService
{
    Task<PagedUserResponse> GetAllUsersAsync(int pageIndex, int pageSize);
    Task<AuthResponse> GetUserByIdAsync(Guid id);
}
