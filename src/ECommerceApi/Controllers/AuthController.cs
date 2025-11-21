using ECommerceApi.Dtos.Auth;
using ECommerceApi.Dtos.Product;
using ECommerceApi.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

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
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(LoginUnauthorizedExample))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.GenerateTokens(request);
            if (response is null)
            {
                return Unauthorized(new
                {
                    status = 401,
                    error = "Unauthorized",
                    message = "Invalid email or password."
                });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RegisterUserAlreadyExistsExample))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterUser(request);

            return Ok(response);
        }
    }
}