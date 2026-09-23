using InventoryDemo.Domain;

namespace InventoryDemo.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Product_DefaultCreatedUtc_IsSet()
    {
        var product = new Product();
        Assert.True(product.CreatedUtc <= DateTime.UtcNow);
        Assert.True(product.CreatedUtc > DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public void Product_Properties_SetAndGet()
    {
        var product = new Product
        {
            Name = "Widget",
            Sku = "WID-001",
            Price = 9.99m,
            Stock = 100,
            CategoryId = 1,
            SupplierId = 2
        };

        Assert.Equal("Widget", product.Name);
        Assert.Equal("WID-001", product.Sku);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(100, product.Stock);
        Assert.Equal(1, product.CategoryId);
        Assert.Equal(2, product.SupplierId);
    }

    [Fact]
    public void Category_Properties_SetAndGet()
    {
        var cat = new Category { Name = "Tools" };
        Assert.Equal("Tools", cat.Name);
        Assert.NotNull(cat.Products);
    }

    [Fact]
    public void Supplier_Properties_SetAndGet()
    {
        var s = new Supplier { Name = "Acme", ContactEmail = "acme@example.com" };
        Assert.Equal("Acme", s.Name);
        Assert.Equal("acme@example.com", s.ContactEmail);
        Assert.NotNull(s.Products);
    }
}
