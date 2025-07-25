using Microsoft.EntityFrameworkCore;

namespace MyApp.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T Get(int id) => _dbSet.Find(id);
        public IEnumerable<T> GetAll() => _dbSet.ToList();
        public void Add(T entity) => _dbSet.Add(entity);
        public void Remove(T entity) => _dbSet.Remove(entity);
        public void Update(T entity) => _dbSet.Update(entity);
    }
}
