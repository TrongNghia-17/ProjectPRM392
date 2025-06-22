namespace BLL.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();

        CreateMap<ProductRequest, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        //CreateMap<Product, ProductResponse>();
        //CreateMap<ProductRequest, Product>()
        //    .ForMember(dest => dest.ProductId, opt => opt.Ignore())
        //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Không áp dụng cho update
        //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}