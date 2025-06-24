namespace DAL.Implements
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ElectronicStoreDbContext _context;
        public OrderRepository( ElectronicStoreDbContext context)
        {
            _context = context;

        }
        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
