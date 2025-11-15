using ECommerceApi.Dtos.Cart;

namespace ECommerceApi.Services.Interfaces
{
    public interface ICartService
    {
        Task<CreateCartResponse> CreateCartAsync(Guid userId);
        Task<AddProductToCartResponse> AddProductToCartAsync(Guid cartId, Guid productId, AddProductToCartRequest request);
        Task<GetCartByIdResponse> GetCartByIdAsync(Guid cartId);
        Task RemoveProductFromCartAsync(Guid cartId, Guid productId);
    }
}