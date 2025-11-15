using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
}