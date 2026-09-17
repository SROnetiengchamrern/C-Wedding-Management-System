using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class ScheduleController : Controller
{
    private readonly ApplicationDbContext _db;
    public ScheduleController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.ScheduleItems
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.StartTime)
            .ToListAsync();
        var wedding = await _db.Weddings.FirstOrDefaultAsync();
        ViewBag.WeddingDate = wedding?.WeddingDate;
        return View(items);
    }

    public IActionResult Create() => View(new ScheduleItem { StartTime = new TimeSpan(14, 0, 0) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleItem model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(ScheduleItem.Wedding));
        ModelState.Remove(nameof(ScheduleItem.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        model.SortOrder = await _db.ScheduleItems.CountAsync() + 1;
        _db.ScheduleItems.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ScheduleItems.FindAsync(id);
        if (item is not null)
        {
            _db.ScheduleItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
