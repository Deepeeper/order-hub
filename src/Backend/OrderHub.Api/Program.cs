using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using OrderHub.Api.Authentication;
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

// Authentication + authorization.
// The DevBypass scheme auto-authenticates every request as a synthetic user.
// Production swaps this for JWT bearer against Entra ID — endpoints already
// declare RequireAuthorization(), so the change is a one-liner.
builder.Services
    .AddAuthentication(DevBypassHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DevBypassHandler>(DevBypassHandler.SchemeName, null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderHubDbContext>();
    DataSeeder.Seed(db);
}

app.MapGet("/", () => Results.Ok(new
{
    service = "OrderHub.Api",
    version = "0.1.0",
    docs = "None yet. Write a CLAUDE.md first :-)"
}));

app.MapOrderEndpoints();
app.MapCustomerEndpoints();
app.MapProductEndpoints();

app.Run();

// Needed by WebApplicationFactory in the tests
public partial class Program { }
