using ECommerceApi.Dtos.Order;
using ECommerceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutAsync([FromBody] CheckoutRequest request)
        {
            var userId = GetUserId();
            var response = await _orderService.CheckoutAsync(request, userId);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderHistoriesAsync()
        {
            var userId = GetUserId();
            var response = await _orderService.GetOrderHistoriesAsync(userId);
            return Ok(response);
        }
    }
}