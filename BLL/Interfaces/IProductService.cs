namespace BLL.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task DeleteAsync(Guid id);
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request);
}