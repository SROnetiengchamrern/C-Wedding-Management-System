using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class WebsiteController : Controller
{
    private readonly ApplicationDbContext _db;
    public WebsiteController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var wedding = await _db.Weddings.Include(w => w.WebsiteSettings).FirstOrDefaultAsync();
        if (wedding is null) return RedirectToAction("Index", "Home");

        if (wedding.WebsiteSettings is null)
        {
            wedding.WebsiteSettings = new WebsiteSettings
            {
                WeddingId = wedding.Id,
                SiteTitle = $"{wedding.Partner1Name} & {wedding.Partner2Name}",
                Slug = $"{wedding.Partner1Name}-{wedding.Partner2Name}".ToLowerInvariant().Replace(" ", "-"),
                WelcomeMessage = "We're getting married! Join us for our celebration.",
                IsPublished = false
            };
            _db.WebsiteSettings.Add(wedding.WebsiteSettings);
            await _db.SaveChangesAsync();
        }

        return View(wedding.WebsiteSettings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(WebsiteSettings model)
    {
        var settings = await _db.WebsiteSettings.FindAsync(model.Id);
        if (settings is null) return NotFound();

        settings.SiteTitle = model.SiteTitle;
        settings.Slug = model.Slug;
        settings.WelcomeMessage = model.WelcomeMessage;
        settings.RsvpUrl = model.RsvpUrl;
        settings.IsPublished = model.IsPublished;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Website settings saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Preview()
    {
        var settings = await _db.WebsiteSettings.Include(w => w.Wedding).FirstOrDefaultAsync();
        return settings is null ? NotFound() : View(settings);
    }
}
