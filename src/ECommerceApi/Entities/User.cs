using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceApi.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}