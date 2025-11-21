using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Auth
{
    public class LoginRequest
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }

    public class LoginRequestExample : IExamplesProvider<LoginRequest>
    {
        public LoginRequest GetExamples()
        {
            return new LoginRequest
            {
                Username = "email@mail.com",
                Password = "password"
            };
        }
    }
}