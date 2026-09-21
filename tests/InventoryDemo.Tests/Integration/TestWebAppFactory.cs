using InventoryDemo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDemo.Tests.Integration;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"Test-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlite($"Data Source={_dbName}.db;Pooling=False"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            var path = $"{_dbName}.db";
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
