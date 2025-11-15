using ECommerceApi.Entities;

namespace ECommerceApi.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductListAsync();
    }
}