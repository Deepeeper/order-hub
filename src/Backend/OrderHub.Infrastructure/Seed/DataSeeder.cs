using OrderHub.Domain.Customers;
using OrderHub.Domain.Orders;
using OrderHub.Domain.Products;
using OrderHub.Infrastructure.Persistence;

namespace OrderHub.Infrastructure.Seed;

public static class DataSeeder
{
    public static void Seed(OrderHubDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Customers.Any()) return;

        var customers = new[]
        {
            new Customer { Name = "Volvo Group Trucks",     Country = "SE", ContactEmail = "purchasing@volvo.com" },
            new Customer { Name = "Scania CV AB",            Country = "SE", ContactEmail = "supply@scania.com" },
            new Customer { Name = "SKF Sverige AB",          Country = "SE", ContactEmail = "orders@skf.com" },
            new Customer { Name = "Daimler Truck AG",        Country = "DE", ContactEmail = "einkauf@daimlertruck.com" },
            new Customer { Name = "Bosch Rexroth",           Country = "DE", ContactEmail = "supply@boschrexroth.com" },
            new Customer { Name = "Sandvik Coromant",        Country = "SE", ContactEmail = "orders@sandvik.com" },
            new Customer { Name = "ABB Robotics",            Country = "SE", ContactEmail = "supply@abb.com" },
            new Customer { Name = "Kongsberg Automotive",    Country = "NO", ContactEmail = "orders@kongsberg.com" }
        };
        db.Customers.AddRange(customers);

        var products = new[]
        {
            new Product { Name = "Round bar SAE 4140",     SteelGrade = "SAE 4140",  DimensionMm = "Ø 50",   PricePerKg = 28.50m },
            new Product { Name = "Round bar SAE 4340",     SteelGrade = "SAE 4340",  DimensionMm = "Ø 65",   PricePerKg = 31.20m },
            new Product { Name = "Round bar 100Cr6",       SteelGrade = "100Cr6",    DimensionMm = "Ø 80",   PricePerKg = 34.80m },
            new Product { Name = "Round bar 42CrMo4",      SteelGrade = "42CrMo4",   DimensionMm = "Ø 100",  PricePerKg = 27.90m },
            new Product { Name = "Flat bar SS2541",        SteelGrade = "SS 2541",   DimensionMm = "40 x 80", PricePerKg = 32.10m },
            new Product { Name = "Hexagonal bar C45",      SteelGrade = "C45",       DimensionMm = "SW 36",  PricePerKg = 25.40m },
            new Product { Name = "Round bar 16MnCr5",      SteelGrade = "16MnCr5",   DimensionMm = "Ø 30",   PricePerKg = 26.30m, Discontinued = true },
            new Product { Name = "Round bar SAE 8620",     SteelGrade = "SAE 8620",  DimensionMm = "Ø 45",   PricePerKg = 27.20m },
        };
        db.Products.AddRange(products);

        db.SaveChanges();

        var rnd = new Random(42);
        var orders = new List<Order>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < 24; i++)
        {
            var cust = customers[rnd.Next(customers.Length)];
            var status = (OrderStatus)rnd.Next(0, 4);
            var created = now.AddDays(-rnd.Next(1, 90));
            var order = new Order
            {
                OrderNumber = $"OH-{2026}{(1000 + i):0000}",
                CustomerId = cust.Id,
                CustomerName = cust.Name,
                Status = status,
                CreatedAt = created,
                DueDate = created.AddDays(rnd.Next(14, 60)),
                Notes = i % 5 == 0 ? "Express delivery requested." : null,
                Lines = new List<OrderLine>()
            };

            var lineCount = rnd.Next(1, 4);
            for (int l = 0; l < lineCount; l++)
            {
                var prod = products[rnd.Next(products.Length)];
                order.Lines.Add(new OrderLine
                {
                    ProductId = prod.Id,
                    ProductName = prod.Name,
                    SteelGrade = prod.SteelGrade,
                    Quantity = rnd.Next(100, 3000),
                    UnitPrice = prod.PricePerKg
                });
            }
            orders.Add(order);
        }

        db.Orders.AddRange(orders);
        db.SaveChanges();
    }
}
