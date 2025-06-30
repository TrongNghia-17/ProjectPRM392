namespace BLL.Interfaces;

public interface IProductService
{
    Task<PagedProductResponse> GetAllAsync(int pageIndex, int pageSize);
    Task<PagedProductResponse> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize);
    Task<PagedProductResponse> SearchByNameAsync(string name, int pageIndex, int pageSize);
    Task<PagedProductResponse> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageIndex, int pageSize);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task DeleteAsync(Guid id);
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request);
}