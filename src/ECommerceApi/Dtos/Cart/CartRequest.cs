using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Cart
{
    public class AddProductToCartRequest
    {
        public int Quantity { get; set; }
    }

    public class AddProductToCartRequestExample : IExamplesProvider<AddProductToCartRequest>
    {
        public AddProductToCartRequest GetExamples()
        {
            return new AddProductToCartRequest
            {
                Quantity = 10
            };
        }
    }
}