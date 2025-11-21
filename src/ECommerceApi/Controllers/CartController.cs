using ECommerceApi.Dtos.Cart;
using ECommerceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using static ECommerceApi.Dtos.Response.RegisterResponseExample;

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
        [ProducesResponseType(typeof(CreateCartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateCartErrorExample))]
        public async Task<IActionResult> CreateCartAsync()
        {
            var userId = GetUserId();

            var response = await _cartService.CreateCartAsync(userId);
            return Ok(response);
        }

        [HttpPost("{cartId}/products/{productId}")]
        [ProducesResponseType(typeof(AddProductToCartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AddProductToCartBadRequestExamples))]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddProductToCartRequest request, Guid cartId, Guid productId)
        {
            var response = await _cartService.AddProductToCartAsync(cartId, productId, request);
            return Ok(response);
        }

        [HttpGet("{cartId}")]
        [ProducesResponseType(typeof(GetCartByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GetCartByIdNotFoundExample), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GetCartByIdNotFoundExample))]
        public async Task<IActionResult> GetCartByIdAsync(Guid cartId)
        {
            var response = await _cartService.GetCartByIdAsync(cartId);
            return Ok(response);
        }

        [HttpDelete("{cartId}/products/{productId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RemoveProductSuccessExample))]

        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RemoveProduct400Examples))]
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