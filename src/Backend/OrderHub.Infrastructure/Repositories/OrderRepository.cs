using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Orders;
using OrderHub.Domain.Orders;
using OrderHub.Infrastructure.Persistence;

namespace OrderHub.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderHubDbContext _db;

    public OrderRepository(OrderHubDbContext db)
    {
        _db = db;
    }

    public async Task<List<Order>> GetAllAsync(OrderStatus? status, CancellationToken ct)
    {
        // KNOWN GAP: the filter bakes in "OR Pending", which means that Pending
        // orders are always returned, regardless of what the caller asked for.
        // This is intentional for the demo. Catching and fixing this bug is one
        // of the aha-moments — Claude finds it by reading endpoint, repo, and the
        // one existing test.
        var orders = await _db.Orders
            .Where(o => status == null || o.Status == status || o.Status == OrderStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

        // KNOWN GAP: N+1 query. Lines are fetched in a loop per order instead of using Include.
        foreach (var order in orders)
        {
            order.Lines = await _db.OrderLines
                .Where(l => l.OrderId == order.Id)
                .ToListAsync(ct);
        }

        return orders;
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order> AddAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        return order;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync(ct);
    }
}
