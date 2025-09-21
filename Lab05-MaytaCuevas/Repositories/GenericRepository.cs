using Lab05_MaytaCuevas;
using Lab05_MaytaCuevas.Models;
using Microsoft.EntityFrameworkCore;


namespace Lab04_MaytaAlberth.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SistemaAcoContex _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SistemaAcoContex context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }


        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        
        
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public IQueryable<T> GetQueryable()
        {
            return _dbSet;
        }
        
    }
}