using BLL.DTOs.OdersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> UpdateOrderAsync(Guid orderId, UpdateOrderRequest request);
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequest request);
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task<List<OrderResponseDto>> GetAllOrdersByUserIdAsync(Guid userId);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId);
        Task<decimal> GetMonthlyRevenueAsync(int month, int year);
        Task<List<OrderItemDto>> GetOrderItemsByOrderIdAsync(Guid orderId);
        Task<Dictionary<int, decimal>> GetMonthlyRevenueAsync(int year);
    }
}
