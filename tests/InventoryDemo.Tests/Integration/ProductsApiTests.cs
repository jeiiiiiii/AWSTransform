using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using InventoryDemo.Domain;

namespace InventoryDemo.Tests.Integration;

public class ProductsApiTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(int categoryId, int supplierId)> CreateDependencies()
    {
        var cat = await _client.PostAsJsonAsync("/api/categories", new Category { Name = $"Cat-{Guid.NewGuid().ToString("N")[..8]}" });
        cat.EnsureSuccessStatusCode();
        var catObj = await cat.Content.ReadFromJsonAsync<Category>();

        var sup = await _client.PostAsJsonAsync("/api/suppliers", new Supplier { Name = $"Sup-{Guid.NewGuid().ToString("N")[..8]}", ContactEmail = "s@s.com" });
        sup.EnsureSuccessStatusCode();
        var supObj = await sup.Content.ReadFromJsonAsync<Supplier>();

        return (catObj!.Id, supObj!.Id);
    }

    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/api/products?page=1&pageSize=5");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("total", out _));
        Assert.True(json.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        var (catId, supId) = await CreateDependencies();
        var sku = $"WID-{Guid.NewGuid().ToString("N")[..8]}";
        var product = new Product { Name = "Widget", Sku = sku, Price = 9.99m, Stock = 10, CategoryId = catId, SupplierId = supId };
        var response = await _client.PostAsJsonAsync("/api/products", product);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateThenGetProduct_RoundTrip()
    {
        var (catId, supId) = await CreateDependencies();
        var sku = $"SKU-{Guid.NewGuid().ToString("N")[..12]}";
        var product = new Product { Name = "Test Product", Sku = sku, Price = 19.99m, Stock = 5, CategoryId = catId, SupplierId = supId };

        var create = await _client.PostAsJsonAsync("/api/products", product);
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);

        var get = await _client.GetAsync($"/api/products/{created.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<Product>();
        Assert.Equal("Test Product", fetched!.Name);
        Assert.Equal(sku, fetched.Sku);
    }

    [Fact]
    public async Task GetProduct_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/products/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent()
    {
        var (catId, supId) = await CreateDependencies();
        var sku = $"D-{Guid.NewGuid().ToString("N")[..10]}";
        var create = await _client.PostAsJsonAsync("/api/products", new Product { Name = "Del", Sku = sku, Price = 1m, Stock = 1, CategoryId = catId, SupplierId = supId });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(created);

        var delete = await _client.DeleteAsync($"/api/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }
}
