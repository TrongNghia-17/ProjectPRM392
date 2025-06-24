using BLL.DTOs.ProductsDTO;

namespace BLL.Implements;

public class ProductService(IProductRepository productRepository, IMapper mapper, ICartItemRepository cartItemRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICartItemRepository _cartItemRepository = cartItemRepository;

    public async Task<PagedProductResponse> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var (products, totalCount) = await _productRepository.GetByCategoryIdAsync(categoryId, pageIndex, pageSize);
        var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);

        return new PagedProductResponse
        {
            Products = productResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<PagedProductResponse> SearchByNameAsync(string name, int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var (products, totalCount) = await _productRepository.SearchByNameAsync(name, pageIndex, pageSize);
        var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);

        return new PagedProductResponse
        {
            Products = productResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<PagedProductResponse> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageIndex, int pageSize)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            throw new ArgumentException("Invalid price range.");
        }
        if (pageIndex < 0)
        {
            throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

        var (products, totalCount) = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice, pageIndex, pageSize);
        var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);

        return new PagedProductResponse
        {
            Products = productResponses,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
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