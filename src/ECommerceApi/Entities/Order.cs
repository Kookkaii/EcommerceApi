namespace ECommerceApi.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public User? User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}