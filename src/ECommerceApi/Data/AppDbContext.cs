using ECommerceApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.UserId).IsUnique();
                entity.HasOne(c => c.User).WithOne().HasForeignKey<Cart>(c => c.UserId);
                entity.Property(c => c.CreatedAt).HasColumnType("timestamp");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.HasOne(ci => ci.Cart).WithMany(c => c.Items).HasForeignKey(ci => ci.CartId);
                entity.HasOne(ci => ci.Product).WithMany().HasForeignKey(ci => ci.ProductId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(c => c.CreatedAt).HasColumnType("timestamp");
            });
            
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
                entity.HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId);
                entity.Property(oi => oi.Price).HasColumnType("decimal(10,2)");
            });
        }
    }
}