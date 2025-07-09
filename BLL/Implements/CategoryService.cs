using BLL.DTOs.CategoriesDTO;

namespace BLL.Implements;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<PagedCategoryResponse> GetAllAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var (categories, totalCount) = await _categoryRepository.GetAllAsync(pageIndex, pageSize);
        var categoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(categories);

        return new PagedCategoryResponse
        {
            Categories = categoryResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} does not exist.");
        }

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var nameExists = await _categoryRepository.ExistsByNameAsync(request.Name);
        if (nameExists)
        {
            throw new ArgumentException("A category with this name already exists.");
        }

        var category = _mapper.Map<Category>(request);
        category.CategoryId = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;

        await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, CategoryRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} does not exist.");
        }

        var nameExistsForOther = await _categoryRepository.ExistsByNameAsync(request.Name) &&
                                !existingCategory.Name.ToLower().Equals(request.Name.ToLower());
        if (nameExistsForOther)
        {
            throw new ArgumentException("A category with this name already exists.");
        }

        var updatedCategory = _mapper.Map(request, existingCategory);
        updatedCategory.CategoryId = id;
        updatedCategory.CreatedAt = existingCategory.CreatedAt;

        await _categoryRepository.UpdateAsync(updatedCategory);
        return _mapper.Map<CategoryResponse>(updatedCategory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} does not exist.");
        }

        await _categoryRepository.DeleteAsync(id);
    }
}
