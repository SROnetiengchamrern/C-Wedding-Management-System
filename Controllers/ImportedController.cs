using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class ImportedController : Controller
{
    private readonly ApplicationDbContext _db;
    public ImportedController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.ImportedRecords.OrderByDescending(i => i.ImportedAt).ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new ImportedRecord { Source = "CSV", RecordType = "Guest" });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ImportedRecord model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(ImportedRecord.Wedding));
        ModelState.Remove(nameof(ImportedRecord.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        model.ImportedAt = DateTime.UtcNow;
        model.Status = "Imported";
        _db.ImportedRecords.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
