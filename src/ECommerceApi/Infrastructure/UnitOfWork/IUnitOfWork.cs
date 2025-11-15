using ECommerceApi.Infrastructure.Repositories;

namespace ECommerceApi.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}