namespace BLL.DTOs.ProductsDTO;

public class ProductResponse
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
}
