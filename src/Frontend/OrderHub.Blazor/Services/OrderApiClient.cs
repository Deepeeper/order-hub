using System.Net.Http.Json;

namespace OrderHub.Blazor.Services;

public class OrderApiClient
{
    private readonly HttpClient _http;

    public OrderApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderSummaryDto>> GetOrdersAsync(OrderStatus? status = null, CancellationToken ct = default)
    {
        var url = status.HasValue ? $"/api/orders?status={status.Value}" : "/api/orders";
        var result = await _http.GetFromJsonAsync<List<OrderSummaryDto>>(url, ct);
        return result ?? new();
    }

    public async Task<OrderDetailDto?> GetOrderAsync(int id, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<OrderDetailDto>($"/api/orders/{id}", ct);
    }

    public async Task<int?> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/orders", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>(cancellationToken: ct);
    }

    public async Task UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct = default)
    {
        var response = await _http.PatchAsJsonAsync($"/api/orders/{id}/status",
            new UpdateStatusRequest(status), ct);
        response.EnsureSuccessStatusCode();
    }
}

public class CustomerApiClient
{
    private readonly HttpClient _http;

    public CustomerApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<CustomerDto>>("/api/customers", ct);
        return result ?? new();
    }
}

public class ProductApiClient
{
    private readonly HttpClient _http;

    public ProductApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetProductsAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<ProductDto>>("/api/products", ct);
        return result ?? new();
    }
}
