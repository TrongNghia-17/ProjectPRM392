namespace DAL.Implements;

public class ProductRepository(ElectronicStoreDbContext context) : IProductRepository
{
    private readonly ElectronicStoreDbContext _context = context;

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchByNameAsync(string name, int pageIndex, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            var allProducts = _context.Products.AsQueryable();
            var totalCount = await allProducts.CountAsync();
            var products = await allProducts
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (products, totalCount);
        }

        var query = _context.Products
            .Where(p => p.Name.ToLower().Contains(name.ToLower()));

        var total = await query.CountAsync();
        var result = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (result, total);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return await _context.Products
            .AnyAsync(p => p.Name.ToLower() == name.ToLower());
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

    public async Task AddAsync(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} does not exist.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}