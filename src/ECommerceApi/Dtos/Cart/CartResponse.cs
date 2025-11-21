using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Cart
{
    public class CreateCartResponse
    {
        public Guid CartId { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class CartItemsInformation
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class AddProductToCartResponse : CartItemsInformation
    {
        public Guid CartId { get; set; }
    }

    public class GetCartByIdResponse
    {
        public Guid CartId { get; set; }
        public List<CartItemsInformation> CartItems { get; set; } = new();
    }

    #region Example swagger request
    public class CreateCartSuccessExample : IExamplesProvider<CreateCartResponse>
    {
        public CreateCartResponse GetExamples()
        {
            return new CreateCartResponse
            {
                CartId = Guid.Parse("2c494921-6082-43da-8c31-44c5ed66bc99"),
                CreatedAt = "2025-01-01 10:00:00.000"
            };
        }
    }
    public class CreateCartErrorExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                status = 400,
                message = "Cart already exists for this user."
            };
        }
    }

    public class AddProductToCartSuccessExample : IExamplesProvider<AddProductToCartResponse>
    {
        public AddProductToCartResponse GetExamples()
        {
            return new AddProductToCartResponse
            {
                CartId = Guid.Parse("2c494921-6082-43da-8c31-44c5ed66bc99"),
                ProductId = Guid.Parse("d6e2a82d-8c37-4030-8682-a585c91ab105"),
                ProductName = "Example Product",
                Quantity = 3
            };
        }
    }
    public class AddProductToCartBadRequestExamples : IMultipleExamplesProvider<object>
    {
        public IEnumerable<SwaggerExample<object>> GetExamples()
        {
            return new List<SwaggerExample<object>>
        {
            SwaggerExample.Create<object>(
                "CartNotFound",
                new { 
                    status = 400, 
                    error = "BadRequest",
                    message = "Cart with ID 2c494921-6082-43da-8c31-44c5ed66bc99 does not exist." 
                    }),
            SwaggerExample.Create<object>(
                "ProductNotFound",
                new { 
                    status = 400, 
                    error = "BadRequest",
                    message = "Product with ID d6e2a82d-8c37-4030-8682-a585c91ab105 does not exist." 
                    }),
            SwaggerExample.Create<object>(
                "OutOfStock",
                new { 
                    status = 400, 
                    error = "BadRequest",
                    message = "Product 'Example Product' is out of stock." }
            )};
        }
    }

    public class GetCartByIdSuccessExample : IExamplesProvider<GetCartByIdResponse>
    {
        public GetCartByIdResponse GetExamples()
        {
            return new GetCartByIdResponse
            {
                CartId = Guid.Parse("2c494921-6082-43da-8c31-44c5ed66bc99"),
                CartItems = new List<CartItemsInformation>
            {
                new CartItemsInformation
                {
                    ProductId = Guid.Parse("b27d59d4-a1f3-4954-b5e7-e8e432f125aa"),
                    ProductName = "product_01",
                    Quantity = 1
                },
                new CartItemsInformation
                {
                    ProductId = Guid.Parse("5b026f5d-90bc-49a9-931f-72ebfb104a6d"),
                    ProductName = "product_02",
                    Quantity = 2
                }
            }
            };
        }
    }
    public class GetCartByIdNotFoundExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                status = 400,
                error = "BadRequest",
                message = "Cart with ID 2c494921-6082-43da-8c31-44c5ed66bc99 does not exist."
            };
        }
    }

    #endregion
}