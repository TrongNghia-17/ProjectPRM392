namespace BLL.DTOs.CategoriesDTO;

public class CategoryRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CategoryResponse
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PagedCategoryResponse
{
    public IEnumerable<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
