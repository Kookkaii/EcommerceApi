using ECommerceApi.Dtos.Cart;

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
}