using ECommerceApi.Services.Implementations;
using ECommerceApi.Services.Interfaces;

namespace ECommerceApi.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}