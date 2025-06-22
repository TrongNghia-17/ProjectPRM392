namespace BLL.DTOs.ProductDTO;

public class PagedProductResponse
{
    public IEnumerable<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}