namespace MyApp.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        int Complete(); // SaveChanges
    }
}
