using ECommerceApi.Dtos.Cart;
using ECommerceApi.Helpers;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Order
{
    public class CheckoutResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemsInformation> OrderItems { get; set; } = new();
    }

    public class OrderHistoryResponse
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemInformation> OrderItems { get; set; } = new();
    }

    public class OrderItemInformation
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    #region Example swagger request
    public class CheckoutSuccessExample : IExamplesProvider<CheckoutResponse>
    {
        public CheckoutResponse GetExamples()
        {
            return new CheckoutResponse
            {
                OrderId = Guid.Parse("a3d69c4b-5f9f-4d7c-b260-b07d95d88262"),
                TotalAmount = 3500.00m,
                OrderItems = new List<CartItemsInformation>
            {
                new CartItemsInformation
                {
                    ProductId = Guid.Parse("d6e2a82d-8c37-4030-8682-a585c91ab105"),
                    ProductName = "Product A",
                    Quantity = 2
                },
                new CartItemsInformation
                {
                    ProductId = Guid.Parse("83cbbc55-a151-4af0-aac2-34a868ee3a06"),
                    ProductName = "Product B",
                    Quantity = 1
                }
            }
            };
        }
    }
    public class CheckoutBadRequestExamples : IMultipleExamplesProvider<object>
    {
        public IEnumerable<SwaggerExample<object>> GetExamples()
        {
            return new List<SwaggerExample<object>>
        {
            SwaggerExample.Create<object>(
                "CartNotFoundOrEmpty", new {
                    status = 400,
                    error = "BadRequest",
                    message = "Cart with ID aaaaaaaa-1111-2222-3333-bbbbbbbbbbbb does not exist."
                    }),
            SwaggerExample.Create<object>(
                "ProductNotFound", new {
                    status = 400,
                    error = "BadRequest",
                    message = "Product with ID 11111111-1111-1111-1111-111111111111 does not exist."
                    }),
            SwaggerExample.Create<object>(
                "OutOfStock", new {
                    status = 400,
                    error = "BadRequest",
                    message = "Product 'Product A' is out of stock."
                    }),
            SwaggerExample.Create<object>(
                "NotEnoughStock",
                new {
                    status = 400,
                    error = "BadRequest",
                    message = "Not enough stock for product 'Product B'. Available: 2"
                    })
        };
        }
    }

    public class OrderHistoriesSuccessExample : IExamplesProvider<List<OrderHistoryResponse>>
    {
        public List<OrderHistoryResponse> GetExamples()
        {
            return new List<OrderHistoryResponse>
        {
            new OrderHistoryResponse
            {
                OrderId = Guid.Parse("a3d69c4b-5f9f-4d7c-b260-b07d95d88262"),
                CreatedAt = DateTime.UtcNow.AddDays(-2).ToBangkokTime(),
                TotalAmount = 1500.00m,
                OrderItems = new List<OrderItemInformation>
                {
                    new OrderItemInformation
                    {
                        ProductId = Guid.Parse("d6e2a82d-8c37-4030-8682-a585c91ab105"),
                        ProductName = "Product A",
                        Quantity = 2,
                        Price = 500.00m
                    },
                    new OrderItemInformation
                    {
                        ProductId = Guid.Parse("83cbbc55-a151-4af0-aac2-34a868ee3a06"),
                        ProductName = "Product B",
                        Quantity = 1,
                        Price = 500.00m
                    }
                }
            },
            new OrderHistoryResponse
            {
                OrderId = Guid.Parse("8feaca68-da02-4efc-b125-5e9007e6bf84"),
                CreatedAt = DateTime.UtcNow.AddDays(-5).ToBangkokTime(),
                TotalAmount = 2000.00m,
                OrderItems = new List<OrderItemInformation>
                {
                    new OrderItemInformation
                    {
                        ProductId = Guid.Parse("071271a0-a62b-411d-98af-b14bab9d00b6"),
                        ProductName = "Product C",
                        Quantity = 4,
                        Price = 500.00m
                    }
                }
            }
        };
        }
    }

    public class OrderHistoriesBadRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                status = 400,
                error = "BadRequest",
                message = "No orders found for this user."
            };
        }
    }
    #endregion
}