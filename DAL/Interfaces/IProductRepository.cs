namespace ProjectPRM392.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Product product);
}