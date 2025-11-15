using ECommerceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductListAsync()
        {
            var response = await _productService.GetProductListAsync();
            return Ok(response);
        }

    }
}