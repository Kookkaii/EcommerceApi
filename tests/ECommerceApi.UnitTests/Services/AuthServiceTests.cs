using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using ECommerceApi.Configurations;
using ECommerceApi.Dtos.Auth;
using ECommerceApi.Dtos.Product;
using ECommerceApi.Entities;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Middlewares;
using ECommerceApi.Services.Implementations;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace ECommerceApi.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRepository<User>> _userRepository;
        private readonly AuthService _authService;
        public AuthServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockUserService = new Mock<IUserService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _userRepository = new Mock<IRepository<User>>();
            _unitOfWork.Setup(u => u.GetRepository<User>()).Returns(_userRepository.Object);
            var jwtSettings = Options.Create(new JwtSettings
            {
                Key = "super_secret_test_key_1234567890",
                Issuer = "test_issuer",
                Audience = "test_audience",
                ExpiryMinutes = 15
            });

            _authService = new AuthService(_mockUserService.Object, jwtSettings, _unitOfWork.Object);
        }

        [Fact]
        public async Task GenerateTokens_ValidUser_ReturnsToken()
        {
            var loginRequest = _fixture.Create<LoginRequest>();
            var user = _fixture.Build<User>()
                               .With(u => u.Email, loginRequest.Username)
                               .With(u => u.FirstName, "test_first_name")
                               .With(u => u.LastName, "test_last_name")
                               .Create();

            _mockUserService.Setup(s => s.GetUserByEmailAndPassword(loginRequest.Username, loginRequest.Password))
                            .ReturnsAsync(user);

            var result = await _authService.GenerateTokens(loginRequest);

            result.ShouldNotBeNull();
            result!.AccessToken.ShouldNotBeNullOrEmpty();
            result.ExpiresInMinutes.ShouldBe(15);

            _mockUserService.Verify(s => s.GetUserByEmailAndPassword(loginRequest.Username, loginRequest.Password), Times.Once);
        }

        [Fact]
        public async Task GenerateTokens_InvalidUser_ReturnsNull()
        {
            var loginRequest = _fixture.Create<LoginRequest>();

            _mockUserService.Setup(s =>
                s.GetUserByEmailAndPassword(loginRequest.Username, loginRequest.Password))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.GenerateTokens(loginRequest);

            // Assert
            result.ShouldBeNull();

            _mockUserService.Verify(s => s.GetUserByEmailAndPassword(loginRequest.Username, loginRequest.Password), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_NewUser_AddsUserSuccessfully()
        {
            // Arrange
            _userRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                           .ReturnsAsync((User?)null);

            var request = _fixture.Create<RegisterRequest>();

            // Act
            var result = await _authService.RegisterUser(request);

            // Assert
            result.ShouldNotBeNull();
            result.Username.ShouldBe(request.Email);
            result.Password.ShouldBe(request.Password);

            _userRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
                u.Email == request.Email &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName
            )), Times.Once);

            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_UserAlreadyExists_ThrowsInvalidRequestException()
        {
            // Arrange
            var existingUser = _fixture.Create<User>();
            _userRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                           .ReturnsAsync(existingUser);

            var request = _fixture.Build<RegisterRequest>()
                                  .With(r => r.Email, existingUser.Email)
                                  .Create();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidRequestException>(() => _authService.RegisterUser(request));
            ex.Message.ShouldBe("User is already registered.");

            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}