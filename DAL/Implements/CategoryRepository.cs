namespace DAL.Implements;

public class CategoryRepository(ElectronicStoreDbContext context) : ICategoryRepository
{
    private readonly ElectronicStoreDbContext _context = context;

    public async Task<(IEnumerable<Category> Categories, int TotalCount)> GetAllAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var query = _context.Categories.AsQueryable();

        var totalCount = await query.CountAsync();
        var categories = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return (categories, totalCount);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task AddAsync(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} does not exist.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
