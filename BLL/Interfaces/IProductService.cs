namespace BLL.Interfaces;

public interface IProductService : IGenericService<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}