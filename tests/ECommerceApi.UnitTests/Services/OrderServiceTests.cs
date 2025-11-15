using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using ECommerceApi.Dtos.Order;
using ECommerceApi.Entities;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using ECommerceApi.Services.Implementations;
using Moq;
using Shouldly;

namespace ECommerceApi.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRepository<Cart>> _cartRepository;
        private readonly Mock<IRepository<Order>> _orderRepository;
        private readonly Mock<IRepository<Product>> _productRepository;
        private readonly OrderService _orderService;
        public OrderServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _unitOfWork = new Mock<IUnitOfWork>();
            _cartRepository = new Mock<IRepository<Cart>>();
            _orderRepository = new Mock<IRepository<Order>>();
            _productRepository = new Mock<IRepository<Product>>();

            _unitOfWork.Setup(u => u.GetRepository<Cart>()).Returns(_cartRepository.Object);
            _unitOfWork.Setup(u => u.GetRepository<Order>()).Returns(_orderRepository.Object);
            _unitOfWork.Setup(u => u.GetRepository<Product>()).Returns(_productRepository.Object);

            _orderService = new OrderService(_unitOfWork.Object);
        }

        [Fact]
        public async Task CheckoutAsync_ValidCart_ReturnsCheckoutResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product = _fixture.Build<Product>()
                .With(p => p.Stock, 5)
                .With(p => p.Price, 100)
                .With(p => p.Name, "test_product")
                .Create();

            var cartItem = _fixture.Build<CartItem>()
                .With(ci => ci.Product, product)
                .With(ci => ci.ProductId, product.Id)
                .With(ci => ci.Quantity, product.Stock)
                .Without(ci => ci.Cart)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem> { cartItem })
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>>()))
                .ReturnsAsync(cart);

            _orderRepository.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _productRepository.Setup(r => r.Update(It.IsAny<Product>()));

            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            var request = new CheckoutRequest { CartId = cartId };

            // Act
            var result = await _orderService.CheckoutAsync(request, userId);

            // Assert
            result.ShouldNotBeNull();
            result.OrderItems[0].ProductName.ShouldBe("test_product");

            _cartRepository.Verify(r => r.Remove(cart), Times.Once);
            _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CheckoutAsync_CartNotFound_ThrowsInvalidRequestException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>>()))
                .ReturnsAsync((Cart?)null);

            var request = new CheckoutRequest { CartId = cartId };

            // Act & Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => _orderService.CheckoutAsync(request, userId));
            result.Message.ShouldBe($"Cart with ID {request.CartId} does not exist.");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>>()), Times.Once);
            _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetOrderHistoriesAsync_ValidUser_ReturnsOrderHistory()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var fakeProduct = _fixture.Build<Product>()
                .With(p => p.Name, "test_product")
                .Create();

            var fakeOrderItem = _fixture.Build<OrderItem>()
                .With(i => i.Product, fakeProduct)
                .With(i => i.ProductId, fakeProduct.Id)
                .Without(i => i.Order)
                .Create();

            var fakeOrder = _fixture.Build<Order>()
                .With(o => o.UserId, userId)
                .With(o => o.Items, new List<OrderItem> { fakeOrderItem })
                .Create();

            var fakeOrders = new List<Order> { fakeOrder };
            _orderRepository.Setup(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>?>())).ReturnsAsync(fakeOrders);

            // Act
            var result = await _orderService.GetOrderHistoriesAsync(userId);

            // Assert
            result.ShouldNotBeEmpty();

            _orderRepository.Verify(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>?>()), Times.Once);
            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>>()), Times.Never);
        }

        [Fact]
        public async Task GetOrderHistoriesAsync_InValidUser_ThrowInvalidRequestExceptions()
        {
            var userId = Guid.NewGuid();
            _orderRepository.Setup(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>())).ReturnsAsync(new List<Order>());

            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => _orderService.GetOrderHistoriesAsync(userId));
            result.Message.ShouldBe("No orders found for this user.");

            _orderRepository.Verify(r => r.GetAllAsync(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}