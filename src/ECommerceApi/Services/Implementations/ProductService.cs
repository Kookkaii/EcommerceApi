using ECommerceApi.Entities;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Services.Interfaces;

namespace ECommerceApi.Services.Implementations
{
    public class ProductService : IProductService
    {
         private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Product>> GetProductListAsync()
        {
            var productRepo = _unitOfWork.GetRepository<Product>();
            return await productRepo.GetAllAsync();
        }
    }
}