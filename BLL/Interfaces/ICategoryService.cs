using BLL.DTOs.CategoriesDTO;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<PagedCategoryResponse> GetAllAsync(int pageIndex, int pageSize);
    Task<CategoryResponse> GetByIdAsync(Guid id);
    Task<CategoryResponse> CreateAsync(CategoryRequest request);
    Task<CategoryResponse> UpdateAsync(Guid id, CategoryRequest request);
    Task DeleteAsync(Guid id);
}
