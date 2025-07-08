using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetAllOrdersByUserIdAsync(Guid userId);
        Task<decimal> GetMonthlyRevenueAsync(int month, int year);
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
    }
}
