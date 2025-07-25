using MyApp.Models;

namespace MyApp.Data
{
    public interface IProductRepository : IRepository<Product>
    {
        // custom method if needed
    }
}
