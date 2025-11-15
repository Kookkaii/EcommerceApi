using ECommerceApi.Data;
using ECommerceApi.Infrastructure.Repositories;

namespace ECommerceApi.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose(); 
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}