using Microsoft.EntityFrameworkCore;
using OrderHub.Api.Endpoints;
using OrderHub.Application.Customers;
using OrderHub.Application.Orders;
using OrderHub.Application.Products;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Repositories;
using OrderHub.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderHubDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("OrderHubDb")
        ?? "Data Source=../../../data/orderhub.db";
    options.UseSqlite(conn);
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderHubDbContext>();
    DataSeeder.Seed(db);
}

app.MapGet("/", () => Results.Ok(new
{
    service = "OrderHub.Api",
    version = "0.1.0",
    docs = "Inga än. Skriv en CLAUDE.md först :-)"
}));

app.MapOrderEndpoints();
app.MapCustomerEndpoints();
app.MapProductEndpoints();

app.Run();

// Behövs för WebApplicationFactory i testerna
public partial class Program { }
