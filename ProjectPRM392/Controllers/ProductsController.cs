namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

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

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var products = await _productService.SearchByNameAsync(name);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        try
        {
            await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
