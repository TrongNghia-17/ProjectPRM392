using BLL.DTOs.CategoriesDTO;

namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _categoryService.GetAllAsync(pageIndex, pageSize);
            return Ok(new
            {
                Status = "Success",
                Data = result.Categories,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpPost]
    //[Authorize(Roles = "Staff")]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = "Staff")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Request body is null.", Status = "Error" });
            }

            var category = await _categoryService.UpdateAsync(id, request);
            return Ok(new { Message = $"Category with ID {id} has been updated successfully.", Status = "Success", Data = category });
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

    [HttpDelete("{id}")]
    //[Authorize(Roles = "Staff")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new { Message = $"Category with ID {id} has been deleted successfully.", Status = "Success" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
    }
}
