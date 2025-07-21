namespace BLL.DTOs.UsersDTO;

public class SelfUpdateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }    
}
