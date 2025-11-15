using ECommerceApi.Dtos.Product;
using ECommerceApi.Entities;

namespace ECommerceApi.Services.Implementations
{
    public interface IUserService
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByEmailAndPassword(string email, string password);
    }
}