namespace DAL.Implements;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{

    public ProductRepository(ElectronicStoreDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync();
        }

        return await _context.Products
            .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            throw new ArgumentException("Invalid price range.");
        }

        return await _context.Products
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync();
    }
}