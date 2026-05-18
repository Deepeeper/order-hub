namespace OrderHub.Blazor.Services;

public enum OrderStatus
{
    Pending = 0,
    InProduction = 1,
    Completed = 2,
    Cancelled = 3
}

public record OrderSummaryDto(
    int Id,
    string OrderNumber,
    string CustomerName,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? DueDate,
    int LineCount);

public record OrderLineDto(
    int Id,
    int ProductId,
    string ProductName,
    string SteelGrade,
    decimal Quantity,
    decimal UnitPrice);

public record OrderDetailDto(
    int Id,
    string OrderNumber,
    int CustomerId,
    string CustomerName,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? DueDate,
    string? Notes,
    List<OrderLineDto> Lines);

public record CreateOrderRequest(
    int CustomerId,
    DateTime? DueDate,
    string? Notes,
    List<CreateOrderLineRequest> Lines);

public record CreateOrderLineRequest(int ProductId, decimal Quantity);

public record UpdateStatusRequest(OrderStatus Status);

public record CustomerDto(int Id, string Name, string Country, string ContactEmail, bool IsActive);

public record ProductDto(int Id, string Name, string SteelGrade, string DimensionMm, decimal PricePerKg, int LeadTimeDays);
