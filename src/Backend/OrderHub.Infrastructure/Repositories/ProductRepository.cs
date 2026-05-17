using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Products;
using OrderHub.Domain.Products;
using OrderHub.Infrastructure.Persistence;

namespace OrderHub.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderHubDbContext _db;

    public ProductRepository(OrderHubDbContext db)
    {
        _db = db;
    }

    public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
        _db.Products.Where(p => !p.Discontinued).OrderBy(p => p.Name).ToListAsync(ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
}
