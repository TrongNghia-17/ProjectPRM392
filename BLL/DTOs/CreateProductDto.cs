using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
    public int Quantity { get; set; }

    [Url(ErrorMessage = "URL hình ảnh không hợp lệ.")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? CategoryId { get; set; }
}
