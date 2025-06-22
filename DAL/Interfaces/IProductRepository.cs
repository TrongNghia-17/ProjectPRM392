namespace DAL.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Products, int TotalCount)> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize);
    Task<(IEnumerable<Product> Products, int TotalCount)> SearchByNameAsync(string name, int pageIndex, int pageSize);
    Task<bool> ExistsByNameAsync(string name);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Product product);
}