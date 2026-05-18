using Microsoft.EntityFrameworkCore;
using OrderHub.Api.Contracts;
using OrderHub.Application.Customers;
using OrderHub.Application.Orders;
using OrderHub.Application.Products;
using OrderHub.Domain.Orders;

namespace OrderHub.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        // GET /api/orders?status=Completed
        group.MapGet("/", async (
            OrderStatus? status,
            IOrderRepository repo,
            CancellationToken ct) =>
        {
            var orders = await repo.GetAllAsync(status, ct);

            var dtos = orders.Select(o => new OrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.CustomerName,
                o.Status,
                o.CreatedAt,
                o.DueDate,
                o.Lines.Count));

            return Results.Ok(dtos);
        });

        // GET /api/orders/{id}
        group.MapGet("/{id:int}", async (int id, IOrderRepository repo, CancellationToken ct) =>
        {
            var order = await repo.GetByIdAsync(id, ct);
            if (order is null) return Results.NotFound();

            var lines = order.Lines
                .Select(l => new OrderLineDto(
                    l.Id, l.ProductId, l.ProductName, l.SteelGrade,
                    l.Quantity, l.UnitPrice,
                    l.Quantity * l.UnitPrice))
                .ToList();

            var dto = new OrderDetailDto(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.CustomerName,
                order.Status,
                order.CreatedAt,
                order.DueDate,
                order.Notes,
                lines,
                lines.Sum(l => l.Amount));

            return Results.Ok(dto);
        });

        // POST /api/orders
        group.MapPost("/", async (
            CreateOrderRequest request,
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            CancellationToken ct) =>
        {
            // Input validation
            var errors = new Dictionary<string, string[]>();
            if (request.CustomerId <= 0)
                errors["customerId"] = new[] { "CustomerId is required." };
            if (request.Lines is null || request.Lines.Count == 0)
                errors["lines"] = new[] { "At least one order line is required." };
            else
            {
                if (request.Lines.Any(l => l.Quantity <= 0))
                    errors["lines.quantity"] = new[] { "All line quantities must be positive." };
                if (request.Lines.Any(l => l.ProductId <= 0))
                    errors["lines.productId"] = new[] { "Every line must reference a valid product." };
            }
            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            var customer = await customerRepo.GetByIdAsync(request.CustomerId, ct);
            if (customer is null)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["customerId"] = new[] { $"Customer {request.CustomerId} not found." }
                });

            var lines = new List<OrderLine>();
            foreach (var l in request.Lines!)
            {
                var product = await productRepo.GetByIdAsync(l.ProductId, ct);
                if (product is null)
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["lines.productId"] = new[] { $"Product {l.ProductId} not found." }
                    });

                lines.Add(new OrderLine
                {
                    ProductId = l.ProductId,
                    ProductName = product.Name,
                    SteelGrade = product.SteelGrade,
                    Quantity = l.Quantity,
                    UnitPrice = product.PricePerKg
                });
            }

            var order = new Order
            {
                OrderNumber = $"OH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CustomerId = request.CustomerId,
                CustomerName = customer.Name,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DueDate = request.DueDate,
                Notes = request.Notes,
                Lines = lines
            };

            await orderRepo.AddAsync(order, ct);
            return Results.Created($"/api/orders/{order.Id}", order.Id);
        }).RequireAuthorization();

        // PATCH /api/orders/{id}/status
        // Requires authentication. In production this would map to a specific
        // Entra ID role/policy (e.g. "OrderManagement"). For the reference app
        // the dev-bypass authentication scheme accepts every request.
        group.MapPatch("/{id:int}/status", async (
            int id,
            UpdateStatusRequest request,
            IOrderRepository repo,
            CancellationToken ct) =>
        {
            var order = await repo.GetByIdAsync(id, ct);
            if (order is null) return Results.NotFound();
            order.Status = request.Status;
            await repo.UpdateAsync(order, ct);
            return Results.NoContent();
        }).RequireAuthorization();

        // KNOWN GAP: no DELETE endpoint exists, even though the UI expects to be able to delete.

        return app;
    }
}
