using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class MenuController : Controller
{
    private readonly ApplicationDbContext _db;
    public MenuController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.MenuItems
            .OrderBy(m => m.Course)
            .ThenBy(m => m.Name)
            .ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new MenuItem());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItem model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(MenuItem.Wedding));
        ModelState.Remove(nameof(MenuItem.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        _db.MenuItems.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item is not null)
        {
            _db.MenuItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
