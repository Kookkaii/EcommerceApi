using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerceApi.Swagger.Filters
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Name == "cartId")
                {
                    parameter.Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "uuid",
                        Example = new OpenApiString("aeb8e755-fc3c-48c1-8e93-4a6f20e9b298")
                    };
                }

                if (parameter.Name == "productId")
                {
                    parameter.Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "uuid",
                        Example = new OpenApiString("81159542-6e8d-4486-8061-4476383cb775")
                    };
                }
            }
        }

    }
}