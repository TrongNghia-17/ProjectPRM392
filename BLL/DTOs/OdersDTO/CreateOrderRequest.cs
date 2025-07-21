using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.OdersDTO
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
    public class OrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
