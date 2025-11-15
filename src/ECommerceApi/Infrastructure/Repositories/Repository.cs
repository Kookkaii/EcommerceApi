using System.Linq.Expressions;
using ECommerceApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public void Update(TEntity entity) => _dbSet.Update(entity);
        public void Remove(TEntity entity) => _dbSet.Remove(entity);
    }
}