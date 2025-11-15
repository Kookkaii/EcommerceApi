using AutoFixture;
using AutoFixture.AutoMoq;
using ECommerceApi.Entities;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Services.Implementations;
using Moq;
using Shouldly;

namespace ECommerceApi.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRepository<Product>> _repository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IRepository<Product>>();

            _unitOfWork.Setup(u => u.GetRepository<Product>()).Returns(_repository.Object);

            _productService = new ProductService(_unitOfWork.Object);
        }

        [Fact]
        public async Task GetProductListAsync_HaveProduct_ReturnAllProducts()
        {
            var fakeProducts = _fixture.CreateMany<Product>(3).ToList();

            _repository.Setup(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>?>())).ReturnsAsync(fakeProducts);

            var result = await _productService.GetProductListAsync();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            _repository.Verify(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>?>()), Times.Once);
        }

        [Fact]
        public async Task GetProductListAsync_NoProductsExist_ReturnEmptyList()
        {
            _repository.Setup(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>?>())).ReturnsAsync(new List<Product>());

            var result = await _productService.GetProductListAsync();

            result.ShouldBeEmpty();
            _repository.Verify(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>?>()), Times.Once);
        }
    }
}