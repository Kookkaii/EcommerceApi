using Microsoft.EntityFrameworkCore;
using ECommerceApi.Configurations;
using ECommerceApi.Data;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Infrastructure.Repositories;

namespace ECommerceApi.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var dbSettings = new DatabaseSettings();
            configuration.GetSection("DatabaseSettings").Bind(dbSettings);

            var connectionString =
                $"Host={dbSettings.Host};Port={dbSettings.Port};Database={dbSettings.Database};Username={dbSettings.Username};Password={dbSettings.Password}";

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}