using InventoryDemo.Data;
using InventoryDemo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryDemo.Api.Controllers;

[Route("Categories")]
public class CategoriesUiController : Controller
{
    private readonly InventoryDbContext _db;
    public CategoriesUiController(InventoryDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index() =>
        View(await _db.Categories.OrderBy(c => c.Name).ToListAsync());

    [HttpGet("Create")]
    public IActionResult Create() => View(new Category());

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        return cat is null ? NotFound() : View(cat);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (!ModelState.IsValid) return View(category);
        _db.Entry(category).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat is not null) { _db.Categories.Remove(cat); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
