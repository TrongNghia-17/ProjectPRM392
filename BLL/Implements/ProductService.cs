namespace BLL.Implements;

public class ProductService(IProductRepository productRepository, IMapper mapper) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;

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

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var nameExists = await _productRepository.ExistsByNameAsync(request.Name);
        if (nameExists)
        {
            throw new ArgumentException("A product with this name already exists.");
        }

        var product = _mapper.Map<Product>(request);
        product.ProductId = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product);
        var response = _mapper.Map<ProductResponse>(product);

        return response;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} does not exist.");
        }

        await _productRepository.DeleteAsync(product.ProductId);
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID  {id}  does not exist.");
        }

        var response = _mapper.Map<ProductResponse>(product);

        return response;
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} does not exist.");
        }

        var nameExistsForOther = await _productRepository.ExistsByNameAsync(request.Name) &&
                                !existingProduct.Name.ToLower().Equals(request.Name.ToLower());
        if (nameExistsForOther)
        {
            throw new ArgumentException("A product with this name already exists.");
        }

        var updatedProduct = _mapper.Map(request, existingProduct);
        updatedProduct.ProductId = id; 
        updatedProduct.CreatedAt = existingProduct.CreatedAt; 
        updatedProduct.IsActive = request.IsActive; 

        await _productRepository.UpdateAsync(updatedProduct);
        var response = _mapper.Map<ProductResponse>(updatedProduct);

        return response;
    }
}