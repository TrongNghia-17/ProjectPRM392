using BLL.DTOs.OdersDTO;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ElectronicStoreDbContext _context;

        public OrderService(IOrderRepository orderRepository, ElectronicStoreDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<OrderResponseDto> UpdateOrderAsync(Guid orderId, UpdateOrderRequest request)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            // Cập nhật thông tin đơn hàng
            order.Status = request.Status ?? order.Status;
            order.ShippingAddress = request.ShippingAddress ?? order.ShippingAddress;

            await _orderRepository.UpdateOrderAsync(order);

            // Trả về DTO
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                Total = order.Total,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = request.UserId,
                OrderDate = DateTime.Now,
                ShippingAddress = request.ShippingAddress,
                Status = "Chưa thanh toán",
                Total = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");

                if (product.Quantity < item.Quantity)
                    throw new Exception($"Product '{product.Name}' does not have enough stock.");

                var orderItem = new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                order.Total += product.Price * item.Quantity;

                product.Quantity -= item.Quantity;

                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create and return the response DTO
            var response = new OrderResponseDto
            {
                OrderId = order.OrderId,
                Total = order.Total,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                   
                }).ToList()
            };

            return response;
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                Total = o.Total,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Price = oi.Price,
                    Status = o.Status,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetAllOrdersByUserIdAsync(userId);

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                Total = o.Total,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                Total = order.Total,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        public async Task<List<OrderItemDto>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(orderId);

            if (orderItems == null || !orderItems.Any())
                return new List<OrderItemDto>();

            return orderItems.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,                
                Price = oi.Price,
                Quantity = oi.Quantity
            }).ToList();
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            return await _orderRepository.GetMonthlyRevenueAsync(month, year);
        }
    }
}
