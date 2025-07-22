using BLL.DTOs.OdersDTO;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> GetUserByIdAsync(Guid id);
    Task<User> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task DeleteUserAsync(Guid id);
    Task SelfUpdateUserAsync(Guid userId, SelfUpdateUserRequest request);
    Task<OrderResponseDto> CreateOrderAsync(Guid userId);
}
