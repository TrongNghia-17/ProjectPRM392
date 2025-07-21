namespace BLL.DTOs.CartItemsDTO;

public class CartItemResponse
{
    public Guid CartItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; }
}

public class PagedCartItemResponse
{
    public IEnumerable<CartItemResponse> CartItems { get; set; } = null!;
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
