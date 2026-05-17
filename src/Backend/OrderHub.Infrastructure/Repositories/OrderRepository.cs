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
        // KÄND BRIST: filtret bygger med "OR Pending" inbakad — vilket innebär att
        // även Pending-ordrar alltid kommer med, oavsett vad anroparen frågar efter.
        // Det här är medvetet för demo. Att uppmärksamma och fixa den här buggen är
        // en av aha-momenten — Claude hittar den genom att läsa både endpoint, repo och
        // den lilla test som finns.
        var orders = await _db.Orders
            .Where(o => status == null || o.Status == status || o.Status == OrderStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

        // KÄND BRIST: N+1 query. Lines hämtas i en loop per order istället för Include.
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
