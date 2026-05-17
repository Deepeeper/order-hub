using OrderHub.Domain.Orders;

namespace OrderHub.Application.Orders;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(OrderStatus? status, CancellationToken ct);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<Order> AddAsync(Order order, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
}
