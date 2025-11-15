using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceApi.Configurations;
using ECommerceApi.Dtos.Auth;
using ECommerceApi.Dtos.Product;
using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserService userService, IOptions<JwtSettings> jwtOptions, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _jwtSettings = jwtOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponse?> GenerateTokens(LoginRequest request)
        {
            var user = await _userService.GetUserByEmailAndPassword(request.Email, request.Password);
            if (user is null)
            {
                return null;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}")
            };

            int expiryMinutes = _jwtSettings.ExpiryMinutes > 0 ? _jwtSettings.ExpiryMinutes : 15;

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse
            {
                AccessToken = accessToken,
                ExpiresInMinutes = expiryMinutes
            };
        }

        public async Task RegisterUser(RegisterRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            var existingUser = await userRepo.GetFirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser is not null)
                throw new InvalidRequestException("User is already registered.");

            var user = new User
            {
                Title = request.Title,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password)
            };

            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}