using System.Reflection;
using ECommerceApi.Swagger.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Helpers.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddCustomSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ECommerce API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Please enter a valid token ex. 'Bearer {token}'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                options.ExampleFilters();
                options.OperationFilter<SwaggerOperationFilter>();
            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}