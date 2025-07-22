using BLL.DTOs.CategoriesDTO;

namespace BLL.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();

        CreateMap<ProductRequest, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl)) // Add this line
            .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt.ToUniversalTime()));

        //CreateMap<User, UserResponse>()
        //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToUniversalTime()));

        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName ?? string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber)) // Cho phép null
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role ?? "User")) // Giá trị mặc định là "User"
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

        CreateMap<SelfUpdateUserRequest, User>()
           .ForMember(dest => dest.UserId, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.Password, opt => opt.Ignore())
           .ForMember(dest => dest.Role, opt => opt.Ignore())
           .ForMember(dest => dest.IsActive, opt => opt.Ignore())
           .ForMember(dest => dest.CartItems, opt => opt.Ignore())
           .ForMember(dest => dest.Orders, opt => opt.Ignore());

        CreateMap<Category, CategoryResponse>();

        CreateMap<CategoryRequest, Category>()
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());
    }
}