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
}