namespace BLL.Implements;

public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Kiểm tra validation thủ công
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");
        if (!new EmailAddressAttribute().IsValid(request.Email))
            throw new ArgumentException("Invalid email address.");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.");

        // Tìm người dùng theo email
        var user = await _userRepository.GetByEmailAsync(request.Email) ?? throw new KeyNotFoundException("User not found.");

        // Kiểm tra mật khẩu
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid password.");

        // Tạo token JWT
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Kiểm tra validation thủ công        
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");
        if (!new EmailAddressAttribute().IsValid(request.Email))
            throw new ArgumentException("Invalid email address.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        // Kiểm tra email đã tồn tại
        if (await _userRepository.GetByEmailAsync(request.Email) != null)
            throw new ArgumentException("Email already exists.");

        // Mã hóa mật khẩu
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Tạo người dùng mới
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            Password = hashedPassword,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Tạo token JWT
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role) 
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
