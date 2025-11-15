using ECommerceApi.Dtos.Cart;
using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using ECommerceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CreateCartResponse> CreateCartAsync(Guid userId)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();

            var existingCart = await cartRepo.GetFirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCart != null)
                throw new InvalidRequestException("Cart already exists for this user.");

            var cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow.ToBangkokTime()
            };

            await cartRepo.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            return new CreateCartResponse
            {
                CartId = cart.Id,
                CreatedAt = cart.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };
        }

        public async Task<AddProductToCartResponse> AddProductToCartAsync(Guid cartId, Guid productId, AddProductToCartRequest request)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var cartItemRepo = _unitOfWork.GetRepository<CartItem>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(c => c.Id == cartId, include: q => q.Include(c => c.Items).ThenInclude(i => i.Product));
            if (cart is null)
            {
                throw new InvalidRequestException($"Cart with ID {cartId} does not exist.");
            }

            var product = await productRepo.GetFirstOrDefaultAsync(p => p.Id == productId);
            if (product is null)
            {
                throw new InvalidRequestException($"Product with ID {productId} does not exist.");
            }
            if (product.Stock <= 0)
            {
                throw new InvalidRequestException($"Product '{product.Name}' is out of stock.");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem is not null)
            {
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.Quantity
                };

                await cartItemRepo.AddAsync(cartItem);
                cart.Items.Add(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            return new AddProductToCartResponse
            {
                CartId = cart.Id,
                ProductId = product.Id,
                ProductName = product.Name ?? string.Empty,
                Quantity = cartItem.Quantity
            };
        }

        public async Task<GetCartByIdResponse> GetCartByIdAsync(Guid cartId)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(c => c.Id == cartId, include: q => q.Include(c => c.Items).ThenInclude(i => i.Product));

            if (cart == null)
            {
                throw new InvalidRequestException($"Cart with ID {cartId} does not exist.");
            }


            return new GetCartByIdResponse
            {
                CartId = cart.Id,
                CartItems = cart.Items.Select(i => new CartItemsInformation
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task RemoveProductFromCartAsync(Guid cartId, Guid productId)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();
            var cartItemRepo = _unitOfWork.GetRepository<CartItem>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(c =>c.Id == cartId,include: q => q.Include(c => c.Items));
            if (cart == null)
            {
                throw new InvalidRequestException($"Cart with ID {cartId} does not exist.");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                throw new InvalidRequestException($"Product with ID {productId} is not in the cart.");
            }
            
            cartItemRepo.Remove(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}