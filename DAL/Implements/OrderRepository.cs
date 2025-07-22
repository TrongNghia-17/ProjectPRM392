namespace DAL.Implements
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ElectronicStoreDbContext _context;
        public OrderRepository( ElectronicStoreDbContext context)
        {
            _context = context;

        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .SumAsync(o => o.Total);
        }

        public async Task<Dictionary<int, decimal>> GetRevenueAsync(int year)
        {
            {
                var monthlyRevenue = await _context.Orders
                    .Where(o => o.OrderDate.Year == year)
                    .GroupBy(o => o.OrderDate.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Revenue = g.Sum(o => o.Total)
                    })
                    .ToDictionaryAsync(x => x.Month, x => x.Revenue);
                for (int month = 1; month <= 12; month++)
                {
                    if (!monthlyRevenue.ContainsKey(month))
                        monthlyRevenue[month] = 0;
                }
                return monthlyRevenue
                    .OrderBy(kv => kv.Key)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }
}
