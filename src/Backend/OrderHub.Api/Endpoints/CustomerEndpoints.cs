using OrderHub.Api.Contracts;
using OrderHub.Application.Customers;

namespace OrderHub.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (ICustomerRepository repo, CancellationToken ct) =>
        {
            var customers = await repo.GetAllAsync(ct);
            return Results.Ok(customers.Select(c =>
                new CustomerDto(c.Id, c.Name, c.Country, c.ContactEmail, c.IsActive)));
        });

        group.MapGet("/{id:int}", async (int id, ICustomerRepository repo, CancellationToken ct) =>
        {
            var c = await repo.GetByIdAsync(id, ct);
            return c is null
                ? Results.NotFound()
                : Results.Ok(new CustomerDto(c.Id, c.Name, c.Country, c.ContactEmail, c.IsActive));
        });

        return app;
    }
}
