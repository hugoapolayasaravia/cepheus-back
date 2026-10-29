using Cepheus.Application.Comun.Interfaces;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public async Task<T?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { code }, cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);
    }

}
