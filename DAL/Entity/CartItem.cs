namespace DAL.Entity;

public partial class CartItem
{
    public Guid CartItemId { get; set; }

    public Guid UserId { get; set; }

    public string? ImageUrl { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
