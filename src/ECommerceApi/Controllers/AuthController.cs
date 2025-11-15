using ECommerceApi.Dtos.Auth;
using ECommerceApi.Dtos.Product;
using ECommerceApi.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.GenerateTokens(request);
            if (response is null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _authService.RegisterUser(request);

            return Ok();
        }
    }
}