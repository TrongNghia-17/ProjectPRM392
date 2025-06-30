namespace BLL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBLLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddScoped<IProductService, ProductService>();
        services.AddAutoMapper(typeof(ProductProfile));
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartItemService, CartItemService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        return services;
    }
}