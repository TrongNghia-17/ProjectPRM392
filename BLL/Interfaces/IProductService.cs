namespace BLL.Interfaces;

public interface IProductService
{
    Task<PagedProductResponse> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize);
    Task<PagedProductResponse> SearchByNameAsync(string name, int pageIndex, int pageSize);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task DeleteAsync(Guid id);
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request);
}