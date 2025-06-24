namespace DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
    {        
        services.AddDbContext<ElectronicStoreDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                x => x.EnableRetryOnFailure()));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        return services;
    }
}