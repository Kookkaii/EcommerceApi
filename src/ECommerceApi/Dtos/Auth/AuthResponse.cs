namespace ECommerceApi.Dtos.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}