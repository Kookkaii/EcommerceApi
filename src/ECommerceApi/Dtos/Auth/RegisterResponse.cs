using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Auth
{
    public class RegisterResponse
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }

    public class RegisterResponseExample : IExamplesProvider<RegisterResponse>
    {
        public RegisterResponse GetExamples()
        {
            return new RegisterResponse
            {
                Username = "username@mail.com",
                Password = "password"
            };
        }
    }
    public class RegisterUserAlreadyExistsExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                status = 400,
                error = "BadRequest",
                message = "User is already registered."
            };
        }
    }
}