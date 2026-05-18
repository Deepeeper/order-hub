namespace OrderHub.Blazor.Services;

public static class OrderStatusExtensions
{
    /// <summary>
    /// Friendly display name for an order status, used everywhere the status is
    /// shown to a user. Keeps wording consistent across tables, badges and dropdowns.
    /// </summary>
    public static string Display(this OrderStatus status) => status switch
    {
        OrderStatus.Pending      => "Pending",
        OrderStatus.InProduction => "In production",
        OrderStatus.Completed    => "Completed",
        OrderStatus.Cancelled    => "Cancelled",
        _ => status.ToString()
    };
}
