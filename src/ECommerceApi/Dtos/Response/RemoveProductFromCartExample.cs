using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Response
{
    public class RegisterResponseExample
    {
        public class RemoveProductSuccessExample : IExamplesProvider<object>
        {
            public object GetExamples()
            {
                return new
                {
                    message = "Product d6e2a82d-8c37-4030-8682-a585c91ab105 has been removed from cart 11111111-1111-1111-1111-111111111111."
                };
            }
        }

        public class RemoveProduct400Examples : IMultipleExamplesProvider<object>
        {
            public IEnumerable<SwaggerExample<object>> GetExamples()
            {
                return new List<SwaggerExample<object>>
                {
                    SwaggerExample.Create<object>(
                        "CartNotFound", new {
                            status = 400,
                            error = "BadRequest",
                            message = "Cart with ID 2c494921-6082-43da-8c31-44c5ed66bc99 does not exist."
                            }),
                    SwaggerExample.Create<object>(
                        "ProductNotInCart", new {
                            status = 400,
                            error = "BadRequest",
                            message = "Product with ID d6e2a82d-8c37-4030-8682-a585c91ab105 is not in the cart."
                            })};
            }
        }
    }
}