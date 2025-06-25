namespace BLL.Interfaces;

public interface IUserService
{
    Task<PagedUserResponse> GetAllUsersAsync(int pageIndex, int pageSize);
    Task<AuthResponse> GetUserByIdAsync(Guid id);
    Task UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task DeleteUserAsync(Guid id);
    Task SelfUpdateUserAsync(Guid userId, SelfUpdateUserRequest request);
}
