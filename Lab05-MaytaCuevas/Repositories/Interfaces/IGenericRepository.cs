namespace Lab05_MaytaCuevas
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(T entity); 
        void Update(T entity); 
        Task DeleteAsync(int id); 
        IQueryable<T> GetQueryable();
    }
}