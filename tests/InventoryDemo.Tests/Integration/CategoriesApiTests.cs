using System.Net;
using System.Net.Http.Json;
using InventoryDemo.Domain;

namespace InventoryDemo.Tests.Integration;

public class CategoriesApiTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public CategoriesApiTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreated()
    {
        var category = new Category { Name = $"TestCat-{Guid.NewGuid().ToString("N")[..8]}" };
        var response = await _client.PostAsJsonAsync("/api/categories", category);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetCategory_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/categories/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateThenGetCategory_RoundTrip()
    {
        var name = $"Round-{Guid.NewGuid().ToString("N")[..8]}";
        var create = await _client.PostAsJsonAsync("/api/categories", new Category { Name = name });
        create.EnsureSuccessStatusCode();

        var created = await create.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(created);

        var get = await _client.GetAsync($"/api/categories/{created!.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<Category>();
        Assert.Equal(name, fetched!.Name);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent()
    {
        var create = await _client.PostAsJsonAsync("/api/categories", new Category { Name = $"Del-{Guid.NewGuid().ToString("N")[..8]}" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<Category>();

        var delete = await _client.DeleteAsync($"/api/categories/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }
}
