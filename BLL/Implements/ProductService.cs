namespace BLL.Implements;

public class ProductService(IProductRepository productRepository, ElectronicStoreDbContext context) : GenericService<Product>(productRepository, context), IProductService
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _productRepository.GetByCategoryIdAsync(categoryId);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        return await _productRepository.SearchByNameAsync(name);
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            throw new ArgumentException("Invalid price range.");
        }
        return await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
    }

    public async Task<Product> CreateAsync(CreateProductDto productDto)
    {
        if (productDto == null)
        {
            throw new ArgumentNullException(nameof(productDto));
        }

        var nameExists = await _productRepository.ExistsByNameAsync(productDto.Name);
        if (nameExists)
        {
            throw new ArgumentException("A product with this name already exists.");
        }

        var product = new Product
        {
            ProductId = Guid.NewGuid(),
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            ImageUrl = productDto.ImageUrl,
            IsActive = productDto.IsActive,
            CategoryId = productDto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        return product;
    }
}