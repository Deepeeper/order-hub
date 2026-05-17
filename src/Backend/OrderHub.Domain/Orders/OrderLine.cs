namespace OrderHub.Domain.Orders;

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // NOTE: Affärslogik (totalpris) räknas idag ut i Blazor-vyn istället för här.
    // Det är en av de medvetna bristerna i refererensappen.
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Snapshot vid orderläggning (produktinfo kan ändras senare)
    public string ProductName { get; set; } = string.Empty;
    public string SteelGrade { get; set; } = string.Empty;
}
