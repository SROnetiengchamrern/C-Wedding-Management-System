using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;
using WeddingManagementSystem.ViewModels;

namespace WeddingManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var couples = await _db.Weddings
            .Include(w => w.WebsiteSettings)
            .Include(w => w.WeddingGifts)
            .OrderBy(w => w.WebId)
            .ToListAsync();

        if (couples.Count == 0)
            return View("Empty");

        var weddingIds = couples.Select(c => c.Id).ToList();
        var rsvps = await _db.WebsiteRsvps
            .Where(r => weddingIds.Contains(r.WeddingId))
            .ToListAsync();

        var rows = couples.Select(w =>
        {
            var coupleRsvps = rsvps.Where(r => r.WeddingId == w.Id).ToList();
            return new DashboardCoupleRow
            {
                WeddingId = w.Id,
                WebId = w.WebId,
                CoupleName = w.CoupleDisplayName,
                WeddingDate = w.WeddingDate,
                DateDisplayShort = w.DateDisplayShort,
                VenueName = w.VenueName,
                IsPublished = w.WebsiteSettings?.IsPublished == true,
                Slug = w.WebsiteSettings?.Slug,
                GiftCount = w.WeddingGifts.Count,
                GiftKhr = w.WeddingGifts.Sum(g => g.AmountKhr),
                GiftUsd = w.WeddingGifts.Sum(g => g.AmountUsd),
                RsvpCount = coupleRsvps.Count,
                DaysLeft = w.DaysLeft
            };
        }).ToList();

        var vm = new DashboardViewModel
        {
            CouplesCount = couples.Count,
            PublishedSites = couples.Count(c => c.WebsiteSettings?.IsPublished == true),
            DraftSites = couples.Count(c => c.WebsiteSettings is null || !c.WebsiteSettings.IsPublished),
            GiftGuests = couples.Sum(c => c.WeddingGifts.Count),
            GiftKhr = couples.Sum(c => c.WeddingGifts.Sum(g => g.AmountKhr)),
            GiftUsd = couples.Sum(c => c.WeddingGifts.Sum(g => g.AmountUsd)),
            RsvpTotal = rsvps.Count,
            RsvpAccepted = rsvps.Count(r => r.Reply == RsvpStatus.Accepted),
            RsvpDeclined = rsvps.Count(r => r.Reply == RsvpStatus.Declined),
            RsvpMaybe = rsvps.Count(r => r.Reply == RsvpStatus.Maybe),
            Couples = rows
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
