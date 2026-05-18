using OrderHub.Blazor.Components;
using OrderHub.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Api:BaseUrl must be configured in appsettings.json.");

void RegisterApiClient<TClient>() where TClient : class
    => builder.Services.AddHttpClient<TClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

RegisterApiClient<OrderApiClient>();
RegisterApiClient<CustomerApiClient>();
RegisterApiClient<ProductApiClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
