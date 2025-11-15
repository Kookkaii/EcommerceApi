using ECommerceApi.Configurations;

namespace ECommerceApi.Extensions
{
    public static class ConfigurationsExtension
    {
        public static IServiceCollection AddAppConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<DatabaseSettings>(configuration.GetSection("ConnectionStrings"));

            return services;
        }
    }
}