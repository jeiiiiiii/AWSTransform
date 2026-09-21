using InventoryDemo.Data;
using InventoryDemo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryDemo.Api.Controllers;

[Route("Products")]
public class ProductsUiController : Controller
{
    private readonly InventoryDbContext _db;
    public ProductsUiController(InventoryDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index(int? category, int? supplier, string? search, int page = 1, int pageSize = 10)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Supplier).AsQueryable();
        if (category.HasValue) query = query.Where(p => p.CategoryId == category);
        if (supplier.HasValue) query = query.Where(p => p.SupplierId == supplier);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));

        ViewBag.Total = await query.CountAsync();
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Search = search;
        ViewBag.CategoryFilter = category;
        ViewBag.SupplierFilter = supplier;
        ViewBag.Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Suppliers = await _db.Suppliers.OrderBy(s => s.Name).ToListAsync();

        var items = await query.OrderBy(p => p.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(items);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new Product());
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(product); }
        product.CreatedUtc = DateTime.UtcNow;
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        await LoadDropdowns();
        return View(product);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(product); }
        var existing = await _db.Products.FindAsync(id);
        if (existing is null) return NotFound();
        existing.Name = product.Name;
        existing.Sku = product.Sku;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.CategoryId = product.CategoryId;
        existing.SupplierId = product.SupplierId;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is not null) { _db.Products.Remove(product); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Categories = new SelectList(await _db.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
        ViewBag.Suppliers = new SelectList(await _db.Suppliers.OrderBy(s => s.Name).ToListAsync(), "Id", "Name");
    }
}
