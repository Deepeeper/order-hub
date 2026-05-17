using OrderHub.Api.Contracts;
using OrderHub.Application.Products;

namespace OrderHub.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (IProductRepository repo, CancellationToken ct) =>
        {
            var products = await repo.GetAllAsync(ct);
            return Results.Ok(products.Select(p =>
                new ProductDto(p.Id, p.Name, p.SteelGrade, p.DimensionMm, p.PricePerKg)));
        });

        group.MapGet("/{id:int}", async (int id, IProductRepository repo, CancellationToken ct) =>
        {
            var p = await repo.GetByIdAsync(id, ct);
            return p is null
                ? Results.NotFound()
                : Results.Ok(new ProductDto(p.Id, p.Name, p.SteelGrade, p.DimensionMm, p.PricePerKg));
        });

        return app;
    }
}
