using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using ECommerceApi.Infrastructure.UnitOfWork;

namespace ECommerceApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            return await userRepo.GetFirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user is not null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }  
    }
}