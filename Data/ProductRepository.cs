using MyApp.Models;

namespace MyApp.Data
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Product> GetPaged(int pageNumber, int pageSize, out int totalItems)
        {
            totalItems = _context.Products.Count();
            return _context.Products
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
