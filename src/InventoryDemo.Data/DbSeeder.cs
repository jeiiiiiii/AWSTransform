using InventoryDemo.Domain;

namespace InventoryDemo.Data;

public static class DbSeeder
{
    public static void Seed(InventoryDbContext context)
    {
        if (context.Categories.Any()) return;

        var electronics = new Category { Name = "Electronics" };
        var furniture = new Category { Name = "Furniture" };
        context.Categories.AddRange(electronics, furniture);

        var techCorp = new Supplier { Name = "TechCorp", ContactEmail = "supply@techcorp.com" };
        var furnishPro = new Supplier { Name = "FurnishPro", ContactEmail = "orders@furnishpro.com" };
        context.Suppliers.AddRange(techCorp, furnishPro);

        context.SaveChanges();

        context.Products.AddRange(
            new Product { Name = "Laptop 15\"", Sku = "ELEC-001", Price = 1299.99m, Stock = 50, CategoryId = electronics.Id, SupplierId = techCorp.Id },
            new Product { Name = "Wireless Mouse", Sku = "ELEC-002", Price = 29.99m, Stock = 200, CategoryId = electronics.Id, SupplierId = techCorp.Id },
            new Product { Name = "Standing Desk", Sku = "FURN-001", Price = 599.00m, Stock = 15, CategoryId = furniture.Id, SupplierId = furnishPro.Id },
            new Product { Name = "Ergonomic Chair", Sku = "FURN-002", Price = 449.00m, Stock = 30, CategoryId = furniture.Id, SupplierId = furnishPro.Id }
        );
        context.SaveChanges();
    }
}
