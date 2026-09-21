using System.Net;
using System.Net.Http.Json;
using InventoryDemo.Domain;

namespace InventoryDemo.Tests.Integration;

public class SuppliersApiTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public SuppliersApiTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSuppliers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/suppliers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateSupplier_ReturnsCreated()
    {
        var supplier = new Supplier { Name = $"Sup-{Guid.NewGuid().ToString("N")[..8]}", ContactEmail = "test@test.com" };
        var response = await _client.PostAsJsonAsync("/api/suppliers", supplier);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateThenGetSupplier_RoundTrip()
    {
        var name = $"Sup-{Guid.NewGuid().ToString("N")[..8]}";
        var create = await _client.PostAsJsonAsync("/api/suppliers", new Supplier { Name = name, ContactEmail = "x@x.com" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<Supplier>();
        Assert.NotNull(created);

        var get = await _client.GetAsync($"/api/suppliers/{created!.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<Supplier>();
        Assert.Equal(name, fetched!.Name);
    }

    [Fact]
    public async Task DeleteSupplier_ReturnsNoContent()
    {
        var create = await _client.PostAsJsonAsync("/api/suppliers", new Supplier { Name = $"Del-{Guid.NewGuid().ToString("N")[..8]}", ContactEmail = "del@test.com" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<Supplier>();

        var delete = await _client.DeleteAsync($"/api/suppliers/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }
}
