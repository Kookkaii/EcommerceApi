using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Product
{
    public class RegisterRequest
    {
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }


    #region Example swagger request
    public class RegisterRequestExample : IExamplesProvider<RegisterRequest>
    {
        public  RegisterRequest GetExamples()
        {
            return new RegisterRequest
            {
                Title = "Miss",
                FirstName = "Kookkaii",
                LastName = "Phalichai",
                Email = "email@mail.com",
                Password = "password"
            };
        }
    }
    #endregion
}