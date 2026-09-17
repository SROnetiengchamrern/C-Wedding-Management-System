using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class DeliveryController : Controller
{
    private readonly ApplicationDbContext _db;
    public DeliveryController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.DeliveryItems.OrderBy(d => d.DeliveryDate).ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new DeliveryItem { DeliveryDate = DateTime.Today.AddDays(7) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeliveryItem model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(DeliveryItem.Wedding));
        ModelState.Remove(nameof(DeliveryItem.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        _db.DeliveryItems.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkDelivered(int id)
    {
        var item = await _db.DeliveryItems.FindAsync(id);
        if (item is not null)
        {
            item.Status = DeliveryStatus.Delivered;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
