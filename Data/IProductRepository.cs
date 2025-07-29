using MyApp.Models;

namespace MyApp.Data
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetPaged(int pageNumber, int pageSize, out int totalItems);
    }
}
