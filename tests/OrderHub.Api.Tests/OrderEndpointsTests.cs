using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace OrderHub.Api.Tests;

// KÄND BRIST: detta är den enda testfilen för API:t. Den täcker bara att roten svarar.
// Order-, Customer- och Product-endpoints saknar tester helt.
public class OrderEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrderEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Root_Endpoint_Returns_Ok()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
