﻿namespace DAL.Implements;

public class UserRepository(ElectronicStoreDbContext context) : IUserRepository
{
    private readonly ElectronicStoreDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

       _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        var users = await _context.
            Users
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();   
        return users;
    }

    public async Task DeleteAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        // Xóa các quan hệ liên quan (CartItems, Orders) để tránh lỗi ràng buộc khóa ngoại
        var cartItems = await _context.CartItems.Where(c => c.UserId == user.UserId).ToListAsync();
        if (cartItems.Any())
        {
            _context.CartItems.RemoveRange(cartItems);
        }

        var orders = await _context.Orders.Where(o => o.UserId == user.UserId).ToListAsync();
        if (orders.Any())
        {
            foreach (var order in orders)
            {
                var orderDetails = await _context.OrderItems.Where(o => o.OrderId == order.OrderId).ToListAsync();
                if (orderDetails.Any())
                {
                    _context.OrderItems.RemoveRange(orderDetails);
                }
            }
            _context.Orders.RemoveRange(orders);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}