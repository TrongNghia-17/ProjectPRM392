namespace BLL.Implements;

public class ProductService : GenericService<Product>, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ElectronicStoreDbContext _context;

    public ProductService(IProductRepository productRepository, ElectronicStoreDbContext context) : base(productRepository, context)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _context = context;
    }

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
}