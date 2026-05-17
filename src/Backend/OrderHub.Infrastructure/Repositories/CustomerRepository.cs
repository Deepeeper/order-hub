using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Customers;
using OrderHub.Domain.Customers;
using OrderHub.Infrastructure.Persistence;

namespace OrderHub.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OrderHubDbContext _db;

    public CustomerRepository(OrderHubDbContext db)
    {
        _db = db;
    }

    public Task<List<Customer>> GetAllAsync(CancellationToken ct) =>
        _db.Customers.OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Customer?> GetByIdAsync(int id, CancellationToken ct) =>
        _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
}
