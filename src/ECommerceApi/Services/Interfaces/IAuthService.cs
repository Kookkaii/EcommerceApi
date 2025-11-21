using ECommerceApi.Dtos.Auth;
using ECommerceApi.Dtos.Product;

namespace ECommerceApi.Services.Implementations
{
    public interface IAuthService
    {
        Task<AuthResponse?> GenerateTokens(LoginRequest request);
        Task<RegisterResponse> RegisterUser(RegisterRequest request);
    }
}