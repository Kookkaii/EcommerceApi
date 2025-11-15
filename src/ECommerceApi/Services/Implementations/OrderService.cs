using ECommerceApi.Data;
using ECommerceApi.Dtos.Cart;
using ECommerceApi.Dtos.Order;
using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using ECommerceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CheckoutResponse> CheckoutAsync(CheckoutRequest request, Guid userId)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var productRepo = _unitOfWork.GetRepository<Product>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.Id == request.CartId && c.UserId == userId,
                include: q => q.Include(c => c.Items).ThenInclude(i => i.Product));

            if (cart is null || !cart.Items.Any())
                throw new InvalidRequestException($"Cart with ID {request.CartId} does not exist.");

            var orderItems = new List<OrderItem>();
            var cartItemInfo = new List<CartItemsInformation>();
            decimal totalAmount = 0;
            foreach (var item in cart.Items.ToList())
            {
                var product = item.Product;
                if (product is null)
                {
                    throw new InvalidRequestException($"Product with ID {item.ProductId} does not exist.");
                }
                if (product.Stock <= 0)
                {
                    throw new InvalidRequestException($"Product '{product.Name}' is out of stock.");
                }
                if (product.Stock < item.Quantity)
                {
                    throw new InvalidRequestException($"Not enough stock for product '{product.Name}'. Available: {product.Stock}");
                }

                var itemPrice = item.Quantity * product.Price;
                totalAmount += itemPrice;
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                });

                product.Stock -= item.Quantity;
                productRepo.Update(product);

                cartItemInfo.Add(new CartItemsInformation
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? string.Empty,
                    Quantity = item.Quantity
                });
            }

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow.ToBangkokTime(),
                TotalAmount = totalAmount,
                Items = orderItems
            };

            cartRepo.Remove(cart);
            await orderRepo.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return new CheckoutResponse
            {
                OrderId = order.Id,
                TotalAmount = totalAmount,
                OrderItems = cartItemInfo
            };
        }

        public async Task<List<OrderHistoryResponse>> GetOrderHistoriesAsync(Guid userId)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var orders = await orderRepo.GetAllAsync(include: q => q.Include(o => o.Items).ThenInclude(i => i.Product));

            var OrderHistories = orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt)
                                .Select(o => new OrderHistoryResponse
                                {
                                    OrderId = o.Id,
                                    CreatedAt = o.CreatedAt,
                                    TotalAmount = o.TotalAmount,
                                    OrderItems = o.Items.Select(i => new OrderItemInformation
                                    {
                                        ProductId = i.ProductId,
                                        ProductName = i.Product!.Name,
                                        Quantity = i.Quantity,
                                        Price = i.Price
                                    }).ToList()
                                }).ToList();

            if (!OrderHistories.Any())
            {
                throw new InvalidRequestException("No orders found for this user.");
            }

            return OrderHistories;
        }
    }
}