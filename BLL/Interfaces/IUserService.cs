using BLL.DTOs.OdersDTO;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<PagedUserResponse> GetAllUsersAsync(int pageIndex, int pageSize);
    Task<UserResponse> GetUserByIdAsync(Guid id);
    Task UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task DeleteUserAsync(Guid id);
    Task SelfUpdateUserAsync(Guid userId, SelfUpdateUserRequest request);
    Task<OrderResponseDto> UpdateUserAndCreateOrderAsync(Guid userId);
}
