using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Infrastructure.UnitOfWork;
using ECommerceApi.Services.Implementations;
using Moq;
using Shouldly;

namespace ECommerceApi.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRepository<User>> _userRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UserService _userService;
        public UserServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _userRepository = new Mock<IRepository<User>>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.Setup(u => u.GetRepository<User>()).Returns(_userRepository.Object);

            _userService = new UserService(_unitOfWork.Object);
        }

        [Fact]
        public async Task GetUserByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = _fixture.Build<User>()
                .With(u => u.Email, email)
                .Create();

            _userRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            result.ShouldNotBeNull();
            result!.Email.ShouldBe(email);

            _userRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmail_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "notfound@example.com";
            _userRepository.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            result.ShouldBeNull();
            _userRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAndPassword_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "StrongPass123";
            var hashedPassword = PasswordHasher.HashPassword(password);

            var user = _fixture.Build<User>()
                .With(u => u.Email, email)
                .With(u => u.PasswordHash, hashedPassword)
                .Create();

            _userRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAndPassword(email, password);

            // Assert
            result.ShouldNotBeNull();
            result!.Email.ShouldBe(email);
            _userRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAndPassword_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";
            var correctPassword = "CorrectPass";
            var wrongPassword = "WrongPass";
            var hashedPassword = PasswordHasher.HashPassword(correctPassword);

            var user = _fixture.Build<User>()
                .With(u => u.Email, email)
                .With(u => u.PasswordHash, hashedPassword)
                .Create();

            _userRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAndPassword(email, wrongPassword);

            // Assert
            result.ShouldBeNull();
            _userRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAndPassword_UserNotFound_ReturnsNull()
        {
            // Arrange
            var email = "notfound@example.com";
            var password = "DoesNotMatter";

            _userRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByEmailAndPassword(email, password);

            // Assert
            result.ShouldBeNull();
            _userRepository.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), null), Times.Once);
        }
    }
}