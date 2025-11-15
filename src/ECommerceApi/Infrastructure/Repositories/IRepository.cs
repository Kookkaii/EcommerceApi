using System.Linq.Expressions;
namespace ECommerceApi.Infrastructure.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}