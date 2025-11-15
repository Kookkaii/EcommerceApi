using ECommerceApi.Dtos.Cart;
using ECommerceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCartAsync()
        {
            var userId = GetUserId();
        
            var response = await _cartService.CreateCartAsync(userId);
            return Ok(response);
        }

        [HttpPost("{cartId}/products/{productId}")]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddProductToCartRequest request, Guid cartId, Guid productId)
        {
            var response = await _cartService.AddProductToCartAsync(cartId, productId, request);
            return Ok(response);
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartByIdAsync(Guid cartId)
        {
            var response = await _cartService.GetCartByIdAsync(cartId);
            return Ok(response);
        }

        [HttpDelete("{cartId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromCart(Guid cartId, Guid productId)
        {
            await _cartService.RemoveProductFromCartAsync(cartId, productId);

            return Ok(new
            {
                message = $"Product {productId} has been removed from cart {cartId}."
            });
        }
    }
}