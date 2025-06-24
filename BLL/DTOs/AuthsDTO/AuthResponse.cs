namespace BLL.DTOs.AuthsDTO;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}