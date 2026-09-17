using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class SettingsController : Controller
{
    private readonly ApplicationDbContext _db;
    public SettingsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var wedding = await _db.Weddings.FirstOrDefaultAsync();
        return wedding is null ? RedirectToAction("Index", "Home") : View(wedding);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Wedding model)
    {
        var wedding = await _db.Weddings.FindAsync(model.Id);
        if (wedding is null) return NotFound();
        wedding.GuestPassword = model.GuestPassword;
        wedding.TotalBudget = model.TotalBudget;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Settings saved.";
        return RedirectToAction(nameof(Index));
    }
}
