namespace BLL.DTOs.AuthsDTO;

public class RegisterRequest
{

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    public string Password { get; set; } = null!;

    [Required]
    public string? FullName { get; set; } = null!;

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Address { get; set; }
}