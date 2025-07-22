using BLL.DTOs.CategoriesDTO;
using BLL.DTOs.OdersDTO;

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

        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            /*.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))*/ // Thêm UserId nếu có trong OrderResponseDto
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => ConvertUtcToVietnamTime(src.OrderDate))) // Dòng quan trọng!
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems)); // Đảm bảo bạn đã có mapping cho OrderItem -> OrderItemDto

        // Mapping cho OrderItem
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.OrderItemId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)) // Cần đảm bảo Product được include khi truy vấn OrderItem
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }

    // Hàm hỗ trợ chuyển đổi từ UTC sang giờ Việt Nam
    private DateTime ConvertUtcToVietnamTime(DateTime utcDateTime)
    {
        // Kiểm tra nếu DateTimeKind là Unspecified, gán nó là Utc
        if (utcDateTime.Kind == DateTimeKind.Unspecified)
        {
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }
        else if (utcDateTime.Kind == DateTimeKind.Local)
        {
            // Nếu không may là Local (tức đã được chuyển đổi sai trước đó), chuyển nó về UTC trước
            utcDateTime = utcDateTime.ToUniversalTime();
        }

        // Tìm múi giờ Việt Nam
        // "SE Asia Standard Time" là ID cho múi giờ UTC+7 trên Windows
        // "Asia/Ho_Chi_Minh" là ID cho múi giờ UTC+7 trên Linux/macOS (và là chuẩn IANA)
        // Render thường chạy trên Linux, nên dùng "Asia/Ho_Chi_Minh" là an toàn nhất.
        // Bạn có thể dùng TimeZoneInfo.GetSystemTimeZones() để xem danh sách các ID có sẵn.
        try
        {
            // Thử lấy múi giờ IANA (Linux/macOS) trước
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietnamTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            // Nếu không tìm thấy múi giờ IANA, thử múi giờ Windows
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietnamTimeZone);
        }
    }
}
