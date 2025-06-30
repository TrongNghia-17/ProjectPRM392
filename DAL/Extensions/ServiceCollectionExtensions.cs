namespace DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, string connectionString)
    {
        //services.AddDbContext<ElectronicStoreDbContext>(options =>
        //    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
        //        x => x.EnableRetryOnFailure()));

        services.AddDbContext<ElectronicStoreDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.EnableRetryOnFailure()));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }
}