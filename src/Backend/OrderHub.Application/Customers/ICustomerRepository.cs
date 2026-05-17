using OrderHub.Domain.Customers;

namespace OrderHub.Application.Customers;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct);
    Task<Customer?> GetByIdAsync(int id, CancellationToken ct);
}
