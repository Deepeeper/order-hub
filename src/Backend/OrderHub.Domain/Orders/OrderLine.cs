namespace OrderHub.Domain.Orders;

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // NOTE: Business logic (line total) is currently computed in the Blazor view
    // instead of here. One of the intentional gaps in this reference app.
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Snapshot at order placement time (product info may change later)
    public string ProductName { get; set; } = string.Empty;
    public string SteelGrade { get; set; } = string.Empty;
}
