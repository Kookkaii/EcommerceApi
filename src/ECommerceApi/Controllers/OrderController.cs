using ECommerceApi.Dtos.Order;
using ECommerceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

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
        [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]

        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CheckoutBadRequestExamples))]
        public async Task<IActionResult> CheckoutAsync([FromBody] CheckoutRequest request)
        {
            var userId = GetUserId();
            var response = await _orderService.CheckoutAsync(request, userId);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(OrderHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(OrderHistoriesBadRequestExample))]
        public async Task<IActionResult> GetOrderHistoriesAsync()
        {
            var userId = GetUserId();
            var response = await _orderService.GetOrderHistoriesAsync(userId);
            return Ok(response);
        }
    }
}