namespace OrderHub.Domain.Products;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SteelGrade { get; set; } = string.Empty;
    public string DimensionMm { get; set; } = string.Empty;
    public decimal PricePerKg { get; set; }

    // Production lead time. Steel can take fractional days through certain
    // heat treatment steps, so this is decimal — not int.
    public decimal LeadTimeDays { get; set; }

    public bool Discontinued { get; set; }
}
