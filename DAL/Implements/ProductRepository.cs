namespace DAL.Implements;

public class ProductRepository(ElectronicStoreDbContext context) : IProductRepository
{
    private readonly ElectronicStoreDbContext _context = context;

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        var query = _context.Products
            .Where(p => p.CategoryId == categoryId);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
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
            .OrderByDescending(p => p.CreatedAt)
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

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageIndex, int pageSize)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            throw new ArgumentException("Invalid price range.");
        }
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var query = _context.Products
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return (products, totalCount);
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

    public async Task<int> GetQuantityByIdAsync(Guid productId)
    {
        var product = await _context.Products
            .Where(p => p.ProductId == productId)
            .Select(p => p.Quantity)
            .FirstOrDefaultAsync();
        return product;
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var query = _context.Products.AsQueryable();
        var totalCount = await query.CountAsync();
        var products = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return (products, totalCount);
    }

}