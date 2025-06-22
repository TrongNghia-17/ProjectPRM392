using BLL.DTOs.ProductDTO;

namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 3)
    {
        try
        {
            var result = await _productService.GetByCategoryIdAsync(categoryId, pageIndex, pageSize);
            return Ok(new
            {
                Status = "Success",
                Data = result.Products,
                Pagination = new
                {
                    result.TotalCount,
                    result.PageIndex,
                    result.PageSize,
                    result.TotalPages
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string? name, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 3)
    {
        try
        {
            var result = await _productService.SearchByNameAsync(name ?? string.Empty, pageIndex, pageSize);
            return Ok(new
            {
                Status = "Success",
                Data = result.Products,
                Pagination = new
                {
                    result.TotalCount,
                    result.PageIndex,
                    result.PageSize,
                    result.TotalPages
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        try
        {
            var product = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, request);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            return Ok(new { Message = $"Product with ID {id} has been deleted successfully.", Status = "Success" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Request body is null.", Status = "Error" });
            }

            var product = await _productService.UpdateAsync(id, request);
            return Ok(new { Message = $"Product with ID {id} has been updated successfully.", Status = "Success", Data = product });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }
}
