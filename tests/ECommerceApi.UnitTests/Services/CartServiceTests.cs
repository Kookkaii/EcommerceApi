using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using ECommerceApi.Dtos.Cart;
using ECommerceApi.Entities;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using ECommerceApi.Services.Implementations;
using Moq;
using Shouldly;

namespace ECommerceApi.UnitTests.Services
{
    public class CartServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRepository<Cart>> _cartRepository;
        private readonly Mock<IRepository<CartItem>> _cartItemRepository;
        private readonly Mock<IRepository<Product>> _productRepository;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _unitOfWork = new Mock<IUnitOfWork>();
            _cartRepository = new Mock<IRepository<Cart>>();
            _cartItemRepository = new Mock<IRepository<CartItem>>();
            _productRepository = new Mock<IRepository<Product>>();
            _unitOfWork.Setup(u => u.GetRepository<Cart>()).Returns(_cartRepository.Object);
            _unitOfWork.Setup(u => u.GetRepository<CartItem>()).Returns(_cartItemRepository.Object);
            _unitOfWork.Setup(u => u.GetRepository<Product>()).Returns(_productRepository.Object);

            _cartService = new CartService(_unitOfWork.Object);
        }

        #region CreateCartAsync
        [Fact]
        public async Task CreateCartAsync_CartDoesNotExist_CreateCart()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), null))
                         .ReturnsAsync((Cart?)null);
            _cartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _cartService.CreateCartAsync(userId);

            // Assert
            result.ShouldNotBeNull();

            _cartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCartAsync_CartExists_ThrowException()
        {
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var cartItem = _fixture.Build<CartItem>()
                .Without(ci => ci.Cart)
                .Create();

            var existingCart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem> { cartItem })
                .Create();

            // Arrange
            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), null))
                         .ReturnsAsync(existingCart);

            // Act & Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => _cartService.CreateCartAsync(existingCart.UserId));
            result.Message.ShouldBe("Cart already exists for this user.");

            _cartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region  GetCartByIdAsync
        [Fact]
        public async Task GetCartByIdAsync_CartExists_ReturnsCartResponse()
        {
            var cartId = Guid.NewGuid();
            var product = _fixture.Build<Product>().With(p => p.Name, "test_product").Create();
            var cartItem = _fixture.Build<CartItem>()
            .Without(ci => ci.Cart)
            .With(ci => ci.Product, product)
            .With(ci => ci.Quantity, 2)
            .Create();

            var existingCart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem> { cartItem })
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                            .ReturnsAsync(existingCart);

            var result = await _cartService.GetCartByIdAsync(cartId);

            // Assert
            result.ShouldNotBeNull();
            result.CartId.ShouldBe(cartId);
            result.CartItems[0].ProductName.ShouldBe("test_product");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()), Times.Once);
        }

        [Fact]
        public async Task GetCartByIdAsync_CartNotFound_ThrowsException()
        {
            // Arrange
            var cartId = Guid.NewGuid();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                            .ReturnsAsync((Cart?)null);

            // Act & Assert
            var result = await Should.ThrowAsync<InvalidRequestException>(() => _cartService.GetCartByIdAsync(cartId));

            result.Message.ShouldBe($"Cart with ID {cartId} does not exist.");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()), Times.Once);
        }
        #endregion

        #region  RemoveProductFromCartAsync
        [Fact]
        public async Task RemoveProductFromCartAsync_CartExistsAndProductExists_ShouldRemoveItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cartItem = _fixture.Build<CartItem>()
                .Without(ci => ci.Cart)
                .With(ci => ci.ProductId, productId)
                .Create();

            var existingCart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem> { cartItem })
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Cart, bool>>>(),
                    It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(existingCart);

            // Act
            await _cartService.RemoveProductFromCartAsync(cartId, productId);

            // Assert
            _cartItemRepository.Verify(r => r.Remove(cartItem), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveProductFromCartAsync_CartDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Cart, bool>>>(),
                    It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync((Cart?)null);

            // Act & Assert
            var result = await Should.ThrowAsync<InvalidRequestException>(() => _cartService.RemoveProductFromCartAsync(cartId, productId));

            result.Message.ShouldBe($"Cart with ID {cartId} does not exist.");

            _cartItemRepository.Verify(r => r.Remove(It.IsAny<CartItem>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RemoveProductFromCartAsync_ProductNotInCart_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Cart, bool>>>(),
                    It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(cart);

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidRequestException>(() => _cartService.RemoveProductFromCartAsync(cartId, productId));

            ex.Message.ShouldBe($"Product with ID {productId} is not in the cart.");

            _cartItemRepository.Verify(r => r.Remove(It.IsAny<CartItem>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region  AddProductToCartAsync
        [Fact]
        public async Task AddProductToCartAsync_CartAndProductExist_ShouldAddItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new AddProductToCartRequest { Quantity = 2 };

            var product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Name, "test_product")
                .With(p => p.Stock, 10)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(cart);
            _productRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null))
                .ReturnsAsync(product);

            // Act
            var result = await _cartService.AddProductToCartAsync(cartId, productId, request);

            // Assert
            result.ShouldNotBeNull();
            result.ProductId.ShouldBe(productId);
            result.CartId.ShouldBe(cartId);
            result.Quantity.ShouldBe(request.Quantity);
            result.ProductName.ShouldBe("test_product");

            _cartItemRepository.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddProductToCartAsync_ProductAlreadyInCart_ShouldIncreaseQuantity()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new AddProductToCartRequest { Quantity = 3 };

            var product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Name, "test_product")
                .With(p => p.Stock, 10)
                .Create();

            var existingItem = _fixture.Build<CartItem>()
                .Without(ci => ci.Cart)
                .With(ci => ci.ProductId, productId)
                .With(ci => ci.Quantity, 2)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem> { existingItem })
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(cart);
            _productRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null))
                .ReturnsAsync(product);

            // Act
            var result = await _cartService.AddProductToCartAsync(cartId, productId, request);

            // Assert
            result.ShouldNotBeNull();
            result.Quantity.ShouldBe(5); // 2 + 3
            _cartItemRepository.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddProductToCartAsync_CartDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new AddProductToCartRequest { Quantity = 1 };

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync((Cart?)null);

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidRequestException>(
                () => _cartService.AddProductToCartAsync(cartId, productId, request));

            ex.Message.ShouldBe($"Cart with ID {cartId} does not exist.");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()), Times.Once);
            _productRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddProductToCartAsync_ProductDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new AddProductToCartRequest { Quantity = 1 };

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(cart);
            _productRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidRequestException>(
                () => _cartService.AddProductToCartAsync(cartId, productId, request));

            ex.Message.ShouldBe($"Product with ID {productId} does not exist.");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()), Times.Once);
            _productRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddProductToCartAsync_ProductOutOfStock_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new AddProductToCartRequest { Quantity = 1 };

            var product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Name, "Out of stock product")
                .With(p => p.Stock, 0)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.Id, cartId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            _cartRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()))
                .ReturnsAsync(cart);
            _productRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null))
                .ReturnsAsync(product);

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidRequestException>(
                () => _cartService.AddProductToCartAsync(cartId, productId, request));

            ex.Message.ShouldBe($"Product 'Out of stock product' is out of stock.");

            _cartRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>?>()), Times.Once);
            _productRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
        #endregion
    }
}