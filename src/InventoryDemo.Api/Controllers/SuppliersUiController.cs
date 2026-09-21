using InventoryDemo.Data;
using InventoryDemo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryDemo.Api.Controllers;

[Route("Suppliers")]
public class SuppliersUiController : Controller
{
    private readonly InventoryDbContext _db;
    public SuppliersUiController(InventoryDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index() =>
        View(await _db.Suppliers.OrderBy(s => s.Name).ToListAsync());

    [HttpGet("Create")]
    public IActionResult Create() => View(new Supplier());

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if (!ModelState.IsValid) return View(supplier);
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);
        return supplier is null ? NotFound() : View(supplier);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if (id != supplier.Id) return BadRequest();
        if (!ModelState.IsValid) return View(supplier);
        _db.Entry(supplier).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);
        if (supplier is not null) { _db.Suppliers.Remove(supplier); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
