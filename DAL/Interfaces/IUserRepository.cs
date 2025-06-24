﻿namespace DAL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(int pageIndex, int pageSize);
}