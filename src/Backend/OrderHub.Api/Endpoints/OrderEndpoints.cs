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

            var dto = new OrderDetailDto(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.CustomerName,
                order.Status,
                order.CreatedAt,
                order.DueDate,
                order.Notes,
                order.Lines.Select(l => new OrderLineDto(
                    l.Id, l.ProductId, l.ProductName, l.SteelGrade, l.Quantity, l.UnitPrice)).ToList());

            return Results.Ok(dto);
        });

        // POST /api/orders
        // KNOWN GAP: no validation. CustomerId 0 or missing, empty Lines, negative Quantity —
        // all accepted without any check.
        group.MapPost("/", async (
            CreateOrderRequest request,
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            CancellationToken ct) =>
        {
            var customer = await customerRepo.GetByIdAsync(request.CustomerId, ct);

            var lines = new List<OrderLine>();
            foreach (var l in request.Lines)
            {
                var product = await productRepo.GetByIdAsync(l.ProductId, ct);
                lines.Add(new OrderLine
                {
                    ProductId = l.ProductId,
                    ProductName = product?.Name ?? "(unknown)",
                    SteelGrade = product?.SteelGrade ?? string.Empty,
                    Quantity = l.Quantity,
                    UnitPrice = product?.PricePerKg ?? 0m
                });
            }

            var order = new Order
            {
                OrderNumber = $"OH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CustomerId = request.CustomerId,
                CustomerName = customer?.Name ?? "(unknown customer)",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DueDate = request.DueDate,
                Notes = request.Notes,
                Lines = lines
            };

            await orderRepo.AddAsync(order, ct);
            return Results.Created($"/api/orders/{order.Id}", order.Id);
        });

        // PATCH /api/orders/{id}/status
        // KNOWN GAP: no authorization. Anyone can change status, including to Cancelled.
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
        });

        // KNOWN GAP: no DELETE endpoint exists, even though the UI expects to be able to delete.

        return app;
    }
}
