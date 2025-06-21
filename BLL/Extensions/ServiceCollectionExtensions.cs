namespace BLL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBLLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

        return services;
    }
}