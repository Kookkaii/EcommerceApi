namespace ECommerceApi.Entities
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public User? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}