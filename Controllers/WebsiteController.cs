using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;
using WeddingManagementSystem.Services;

namespace WeddingManagementSystem.Controllers;

public class WebsiteController : Controller
{
    private const long MaxImageBytes = 900 * 1024;
    private const int MaxBankQr = 5;
    private const int MaxGallery = 15;
    private const int MaxCover = 8;

    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ITelegramAlertService _telegram;

    public WebsiteController(ApplicationDbContext db, IWebHostEnvironment env, ITelegramAlertService telegram)
    {
        _db = db;
        _env = env;
        _telegram = telegram;
    }

    public async Task<IActionResult> Index()
    {
        var weddings = await _db.Weddings
            .Include(w => w.WebsiteSettings)
            .OrderBy(w => w.Partner1Name)
            .ThenBy(w => w.Partner2Name)
            .ToListAsync();
        return View(weddings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePartner(
        string partner1Name,
        string partner2Name,
        DateTime? weddingDate,
        string? venueName,
        string? venueLocation,
        string? slug)
    {
        partner1Name = partner1Name?.Trim() ?? "";
        partner2Name = partner2Name?.Trim() ?? "";
        venueName = string.IsNullOrWhiteSpace(venueName) ? null : venueName.Trim();
        venueLocation = string.IsNullOrWhiteSpace(venueLocation) ? null : venueLocation.Trim();

        if (string.IsNullOrWhiteSpace(partner1Name) || string.IsNullOrWhiteSpace(partner2Name))
        {
            TempData["Error"] = "Partner 1 and Partner 2 names are required.";
            return RedirectToAction(nameof(Index));
        }

        if (partner1Name.Length > 100) partner1Name = partner1Name[..100];
        if (partner2Name.Length > 100) partner2Name = partner2Name[..100];
        if (venueName is { Length: > 200 }) venueName = venueName[..200];
        if (venueLocation is { Length: > 200 }) venueLocation = venueLocation[..200];

        var date = weddingDate?.Date ?? DateTime.Today.AddMonths(6);

        var wedding = new Wedding
        {
            Partner1Name = partner1Name,
            Partner2Name = partner2Name,
            WeddingDate = date,
            VenueName = venueName,
            VenueLocation = venueLocation,
            TotalBudget = 0,
            WebId = await NextWebIdAsync()
        };
        _db.Weddings.Add(wedding);
        await _db.SaveChangesAsync();

        var baseSlug = NormalizeSlug(slug) ??
                       NormalizeSlug($"{partner1Name}-{partner2Name}") ??
                       $"wedding-{wedding.Id}";
        var uniqueSlug = await EnsureUniqueSlugAsync(baseSlug, null);

        _db.WebsiteSettings.Add(new WebsiteSettings
        {
            WeddingId = wedding.Id,
            SiteTitle = $"{partner1Name} & {partner2Name}",
            Slug = uniqueSlug,
            WelcomeMessage = $"We're getting married! Join {partner1Name} & {partner2Name} for our celebration.",
            ShareDescription = $"You're invited to celebrate with {partner1Name} & {partner2Name}.",
            InviteMessage = "We can't wait to celebrate with you.",
            ThemeLayout = 6,
            EffectStyle = InviteEffects.HeartsSnow,
            ShowMap = true,
            MapSearch = venueLocation ?? venueName,
            IsPublished = false
        });
        await _db.SaveChangesAsync();

        TempData["Ok"] = $"Added {partner1Name} & {partner2Name}. You can edit their invite site now.";
        return RedirectToAction(nameof(Edit), new { weddingId = wedding.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int weddingId)
    {
        var settings = await GetOrCreateSettingsAsync(weddingId);
        if (settings is null) return NotFound();

        settings.IsPublished = !settings.IsPublished;
        await _db.SaveChangesAsync();

        var wedding = await _db.Weddings.FindAsync(weddingId);
        var names = wedding is null
            ? "This site"
            : $"{wedding.Partner1Name} & {wedding.Partner2Name}";
        TempData["Ok"] = settings.IsPublished
            ? $"{names} is now Published. Guests can open the public link."
            : $"{names} is now a Draft. The public link is hidden.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int weddingId)
    {
        var settings = await GetOrCreateSettingsAsync(weddingId);
        if (settings is null) return NotFound();

        await LoadMediaAsync(settings);
        await _db.Entry(settings).Reference(s => s.Wedding).LoadAsync();
        ViewBag.PublicUrl = BuildPublicUrl(settings.Slug);
        ViewBag.Themes = InviteThemes.All;
        ViewBag.Rsvps = await _db.WebsiteRsvps
            .Where(r => r.WeddingId == settings.WeddingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return View(settings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        WebsiteSettings model,
        IFormFile? shareImageFile,
        IFormFile? partner1PhotoFile,
        IFormFile? partner2PhotoFile)
    {
        var settings = await _db.WebsiteSettings.FindAsync(model.Id);
        if (settings is null) return NotFound();

        settings.SiteTitle = model.SiteTitle;
        settings.Slug = await EnsureUniqueSlugAsync(NormalizeSlug(model.Slug), settings.Id);
        settings.WelcomeMessage = model.WelcomeMessage;
        settings.WelcomeMessageKh = model.WelcomeMessageKh;
        settings.RsvpUrl = model.RsvpUrl;
        settings.ShareDescription = model.ShareDescription;
        settings.ThemeLayout = model.ThemeLayout is >= 1 and <= 7 ? model.ThemeLayout : settings.ThemeLayout;
        settings.ShowMap = string.Equals(Request.Form["ShowMap"], "true", StringComparison.OrdinalIgnoreCase);
        settings.MapSearch = model.MapSearch;
        settings.InviteMessage = model.InviteMessage;
        settings.InviteMessageKh = model.InviteMessageKh;
        settings.DressCode = model.DressCode;
        settings.DressCodeKh = model.DressCodeKh;
        settings.ScheduleText = model.ScheduleText;
        settings.ScheduleTextKh = model.ScheduleTextKh;
        settings.Partner1Title = Truncate(model.Partner1Title, 40);
        settings.Partner2Title = Truncate(model.Partner2Title, 40);
        settings.Partner1BirthDate = model.Partner1BirthDate;
        settings.Partner2BirthDate = model.Partner2BirthDate;
        settings.Partner1Bio = Truncate(model.Partner1Bio, 1000);
        settings.Partner1BioKh = Truncate(model.Partner1BioKh, 1000);
        settings.Partner2Bio = Truncate(model.Partner2Bio, 1000);
        settings.Partner2BioKh = Truncate(model.Partner2BioKh, 1000);
        settings.MusicUrl = model.MusicUrl;
        settings.IsPublished = model.IsPublished;

        settings.ShareImageUrl = await ResolveImageAsync(
            shareImageFile, model.ShareImageUrl, settings.ShareImageUrl, "share");
        settings.Partner1PhotoUrl = await ResolveImageAsync(
            partner1PhotoFile, model.Partner1PhotoUrl, settings.Partner1PhotoUrl, "couple");
        settings.Partner2PhotoUrl = await ResolveImageAsync(
            partner2PhotoFile, model.Partner2PhotoUrl, settings.Partner2PhotoUrl, "couple");

        await SyncCoverImageUrlAsync(settings);

        if (string.IsNullOrWhiteSpace(settings.ShareImageUrl) && !string.IsNullOrWhiteSpace(settings.CoverImageUrl))
            settings.ShareImageUrl = settings.CoverImageUrl;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Website settings saved.";
        return BackToEdit(settings.WeddingId);
    }

    private static string? Truncate(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        return value.Length > max ? value[..max] : value;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTheme(int id, int themeLayout)
    {
        var settings = await _db.WebsiteSettings.FindAsync(id);
        if (settings is null) return NotFound();
        settings.ThemeLayout = themeLayout is >= 1 and <= 7 ? themeLayout : 6;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Theme saved.";
        return BackToEdit(settings.WeddingId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEffect(int id, int effectStyle)
    {
        var settings = await _db.WebsiteSettings.FindAsync(id);
        if (settings is null) return NotFound();
        settings.EffectStyle = InviteEffects.IsValid(effectStyle) ? effectStyle : InviteEffects.None;
        await _db.SaveChangesAsync();
        TempData["Ok"] = settings.EffectStyle == InviteEffects.None
            ? "Invite effect turned off."
            : $"Invite effect saved: {InviteEffects.Get(settings.EffectStyle).Name}.";
        return BackToEdit(settings.WeddingId);
    }

    public async Task<IActionResult> ExportRsvpExcel(int weddingId)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var rsvps = await _db.WebsiteRsvps.AsNoTracking()
            .Where(r => r.WeddingId == weddingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("No,Name,Email,Reply,Message,Dietary,When (UTC)");
        var i = 1;
        foreach (var r in rsvps)
        {
            sb.AppendLine(string.Join(",",
                i++,
                Csv(r.FullName),
                Csv(r.Email),
                Csv(RsvpReplyLabel(r.Reply)),
                Csv(r.Message),
                Csv(r.DietaryNotes),
                r.CreatedAt.ToString("yyyy-MM-dd HH:mm")));
        }

        sb.AppendLine();
        sb.AppendLine($"Accepted,{rsvps.Count(r => r.Reply == RsvpStatus.Accepted)}");
        sb.AppendLine($"Declined,{rsvps.Count(r => r.Reply == RsvpStatus.Declined)}");
        sb.AppendLine($"Maybe,{rsvps.Count(r => r.Reply == RsvpStatus.Maybe)}");
        sb.AppendLine($"Total,{rsvps.Count}");

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        var slug = $"{wedding.Partner1Name}-{wedding.Partner2Name}".ToLowerInvariant().Replace(' ', '-');
        return File(bytes, "text/csv", $"rsvp-{slug}-{DateTime.Today:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> ExportRsvpPdf(int weddingId)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var rsvps = await _db.WebsiteRsvps.AsNoTracking()
            .Where(r => r.WeddingId == weddingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        ViewBag.CoupleName = wedding.CoupleDisplayName;
        ViewBag.WebId = wedding.WebId;
        ViewBag.Accepted = rsvps.Count(r => r.Reply == RsvpStatus.Accepted);
        ViewBag.Declined = rsvps.Count(r => r.Reply == RsvpStatus.Declined);
        ViewBag.Maybe = rsvps.Count(r => r.Reply == RsvpStatus.Maybe);
        return View(rsvps);
    }

    private static string RsvpReplyLabel(RsvpStatus reply) => reply switch
    {
        RsvpStatus.Accepted => "Joyfully accepts",
        RsvpStatus.Declined => "Regretfully declines",
        RsvpStatus.Maybe => "Not sure yet",
        _ => reply.ToString()
    };

    private static string Csv(string? value)
    {
        value ??= "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadBankQr(int id, List<IFormFile>? files)
    {
        var settings = await _db.WebsiteSettings.Include(s => s.BankQrs).FirstOrDefaultAsync(s => s.Id == id);
        if (settings is null) return NotFound();

        var existing = settings.BankQrs.Count;
        if (files is null || files.Count == 0)
        {
            TempData["Error"] = "Choose at least one image.";
            return BackToEdit(settings.WeddingId);
        }

        var order = settings.BankQrs.Any() ? settings.BankQrs.Max(q => q.SortOrder) + 1 : 1;
        var added = 0;
        foreach (var file in files)
        {
            if (existing + added >= MaxBankQr) break;
            var saved = await SaveImageAsync(file, "bank-qr");
            if (saved is null) continue;
            settings.BankQrs.Add(new WebsiteBankQr
            {
                FilePath = saved,
                Label = $"QR {existing + added + 1}",
                SortOrder = order++
            });
            added++;
        }

        await _db.SaveChangesAsync();
        TempData["Ok"] = added > 0 ? $"Added {added} bank QR image(s)." : "No valid images uploaded (PNG/JPEG, max ~900KB).";
        return BackToEdit(settings.WeddingId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveBankQr(int id, string direction)
    {
        var item = await _db.WebsiteBankQrs.FindAsync(id);
        if (item is null) return NotFound();

        var settings = await _db.WebsiteSettings.FindAsync(item.WebsiteSettingsId);
        var list = await _db.WebsiteBankQrs
            .Where(x => x.WebsiteSettingsId == item.WebsiteSettingsId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
        var idx = list.FindIndex(x => x.Id == id);
        var target = direction == "up" ? idx - 1 : idx + 1;
        if (idx >= 0 && target >= 0 && target < list.Count)
            (list[idx].SortOrder, list[target].SortOrder) = (list[target].SortOrder, list[idx].SortOrder);

        await _db.SaveChangesAsync();
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBankQr(int id)
    {
        var item = await _db.WebsiteBankQrs.FindAsync(id);
        if (item is null) return NotFound();
        var settings = await _db.WebsiteSettings.FindAsync(item.WebsiteSettingsId);
        DeletePhysical(item.FilePath);
        _db.WebsiteBankQrs.Remove(item);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Bank QR removed.";
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAllBankQr(int settingsId)
    {
        var settings = await _db.WebsiteSettings.FindAsync(settingsId);
        if (settings is null) return NotFound();
        var items = await _db.WebsiteBankQrs.Where(x => x.WebsiteSettingsId == settingsId).ToListAsync();
        foreach (var item in items)
            DeletePhysical(item.FilePath);
        _db.WebsiteBankQrs.RemoveRange(items);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "All bank QR images removed.";
        return BackToEdit(settings.WeddingId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadGallery(int id, List<IFormFile>? files)
    {
        var settings = await _db.WebsiteSettings.Include(s => s.GalleryPhotos).FirstOrDefaultAsync(s => s.Id == id);
        if (settings is null) return NotFound();

        var existing = settings.GalleryPhotos.Count;
        if (files is null || files.Count == 0)
        {
            TempData["Error"] = "Choose at least one image.";
            return BackToEdit(settings.WeddingId);
        }

        var order = settings.GalleryPhotos.Any() ? settings.GalleryPhotos.Max(q => q.SortOrder) + 1 : 1;
        var added = 0;
        foreach (var file in files)
        {
            if (existing + added >= MaxGallery) break;
            var saved = await SaveImageAsync(file, "gallery");
            if (saved is null) continue;
            settings.GalleryPhotos.Add(new WebsiteGalleryPhoto
            {
                FilePath = saved,
                Label = $"Photo {existing + added + 1}",
                SortOrder = order++
            });
            added++;
        }

        await _db.SaveChangesAsync();
        TempData["Ok"] = added > 0 ? $"Added {added} gallery photo(s)." : "No valid images uploaded (PNG/JPEG, max ~900KB).";
        return BackToEdit(settings.WeddingId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveGallery(int id, string direction)
    {
        var item = await _db.WebsiteGalleryPhotos.FindAsync(id);
        if (item is null) return NotFound();
        var settings = await _db.WebsiteSettings.FindAsync(item.WebsiteSettingsId);

        var list = await _db.WebsiteGalleryPhotos
            .Where(x => x.WebsiteSettingsId == item.WebsiteSettingsId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
        var idx = list.FindIndex(x => x.Id == id);
        var target = direction == "up" ? idx - 1 : idx + 1;
        if (idx >= 0 && target >= 0 && target < list.Count)
            (list[idx].SortOrder, list[target].SortOrder) = (list[target].SortOrder, list[idx].SortOrder);

        await _db.SaveChangesAsync();
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveGallery(int id)
    {
        var item = await _db.WebsiteGalleryPhotos.FindAsync(id);
        if (item is null) return NotFound();
        var settings = await _db.WebsiteSettings.FindAsync(item.WebsiteSettingsId);
        DeletePhysical(item.FilePath);
        _db.WebsiteGalleryPhotos.Remove(item);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Photo removed.";
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadCover(int id, List<IFormFile>? files)
    {
        var settings = await _db.WebsiteSettings.Include(s => s.CoverPhotos).FirstOrDefaultAsync(s => s.Id == id);
        if (settings is null) return NotFound();

        var existing = settings.CoverPhotos.Count;
        if (files is null || files.Count == 0)
        {
            TempData["Error"] = "Choose at least one image.";
            return BackToEdit(settings.WeddingId);
        }

        var order = settings.CoverPhotos.Any() ? settings.CoverPhotos.Max(q => q.SortOrder) + 1 : 1;
        var added = 0;
        foreach (var file in files)
        {
            if (existing + added >= MaxCover) break;
            var saved = await SaveImageAsync(file, "cover");
            if (saved is null) continue;
            settings.CoverPhotos.Add(new WebsiteCoverPhoto
            {
                FilePath = saved,
                Label = $"Cover {existing + added + 1}",
                SortOrder = order++
            });
            added++;
        }

        await SyncCoverImageUrlAsync(settings);
        await _db.SaveChangesAsync();
        TempData["Ok"] = added > 0 ? $"Added {added} cover image(s)." : "No valid images uploaded (PNG/JPEG, max ~900KB).";
        return BackToEdit(settings.WeddingId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveCover(int id, string direction)
    {
        var item = await _db.WebsiteCoverPhotos.FindAsync(id);
        if (item is null) return NotFound();

        var settings = await _db.WebsiteSettings.Include(s => s.CoverPhotos).FirstOrDefaultAsync(s => s.Id == item.WebsiteSettingsId);
        var list = await _db.WebsiteCoverPhotos
            .Where(x => x.WebsiteSettingsId == item.WebsiteSettingsId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
        var idx = list.FindIndex(x => x.Id == id);
        var target = direction == "up" ? idx - 1 : idx + 1;
        if (idx >= 0 && target >= 0 && target < list.Count)
            (list[idx].SortOrder, list[target].SortOrder) = (list[target].SortOrder, list[idx].SortOrder);

        if (settings is not null)
            await SyncCoverImageUrlAsync(settings);
        await _db.SaveChangesAsync();
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveCover(int id)
    {
        var item = await _db.WebsiteCoverPhotos.FindAsync(id);
        if (item is null) return NotFound();
        var settings = await _db.WebsiteSettings.Include(s => s.CoverPhotos).FirstOrDefaultAsync(s => s.Id == item.WebsiteSettingsId);
        DeletePhysical(item.FilePath);
        settings?.CoverPhotos.Remove(item);
        _db.WebsiteCoverPhotos.Remove(item);
        if (settings is not null)
            await SyncCoverImageUrlAsync(settings);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cover image removed.";
        return BackToEdit(settings?.WeddingId ?? 0);
    }

    public async Task<IActionResult> Preview(int weddingId)
    {
        var settings = await _db.WebsiteSettings
            .Include(w => w.Wedding)
            .Include(w => w.BankQrs)
            .Include(w => w.GalleryPhotos)
            .Include(w => w.CoverPhotos)
            .FirstOrDefaultAsync(w => w.WeddingId == weddingId);
        if (settings is null) return NotFound();
        settings.BankQrs = settings.BankQrs.OrderBy(x => x.SortOrder).ToList();
        settings.GalleryPhotos = settings.GalleryPhotos.OrderBy(x => x.SortOrder).ToList();
        settings.CoverPhotos = settings.CoverPhotos.OrderBy(x => x.SortOrder).ToList();
        await EnsureLegacyCoverAsync(settings);
        return View("Public", settings);
    }

    [AllowAnonymous]
    [HttpGet("/w/{slug}")]
    public async Task<IActionResult> Public(string slug)
    {
        var settings = await _db.WebsiteSettings
            .Include(w => w.Wedding)
            .Include(w => w.BankQrs)
            .Include(w => w.GalleryPhotos)
            .Include(w => w.CoverPhotos)
            .FirstOrDefaultAsync(w => w.Slug == slug);

        if (settings is null || !settings.IsPublished) return NotFound();

        settings.BankQrs = settings.BankQrs.OrderBy(x => x.SortOrder).ToList();
        settings.GalleryPhotos = settings.GalleryPhotos.OrderBy(x => x.SortOrder).ToList();
        settings.CoverPhotos = settings.CoverPhotos.OrderBy(x => x.SortOrder).ToList();
        await EnsureLegacyCoverAsync(settings);
        return View(settings);
    }

    [AllowAnonymous]
    [HttpGet("/w/{slug}/gallery")]
    public async Task<IActionResult> PublicGallery(string slug)
    {
        var settings = await _db.WebsiteSettings
            .Include(w => w.Wedding)
            .Include(w => w.GalleryPhotos)
            .FirstOrDefaultAsync(w => w.Slug == slug);

        if (settings is null || !settings.IsPublished) return NotFound();

        settings.GalleryPhotos = settings.GalleryPhotos.OrderBy(x => x.SortOrder).ToList();
        if (!settings.GalleryPhotos.Any()) return RedirectToAction(nameof(Public), new { slug });

        ViewBag.BackUrl = $"/w/{settings.Slug}#gallery";
        return View(settings);
    }

    public async Task<IActionResult> PreviewGallery(int weddingId)
    {
        var settings = await GetOrCreateSettingsAsync(weddingId);
        if (settings is null) return NotFound();

        await LoadMediaAsync(settings);
        await _db.Entry(settings).Reference(s => s.Wedding).LoadAsync();
        if (!settings.GalleryPhotos.Any())
            return RedirectToAction(nameof(Preview), new { weddingId });

        ViewBag.BackUrl = $"/Website/Preview?weddingId={weddingId}#gallery";
        return View("PublicGallery", settings);
    }

    [AllowAnonymous]
    [HttpPost("/w/{slug}/rsvp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitRsvp(
        string slug,
        string? fullName,
        string? email,
        RsvpStatus reply,
        string? message,
        string? dietaryNotes,
        string? returnUrl)
    {
        var settings = await _db.WebsiteSettings.FirstOrDefaultAsync(w => w.Slug == slug);
        if (settings is null) return NotFound();

        fullName = fullName?.Trim();
        email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        message = string.IsNullOrWhiteSpace(message) ? null : message.Trim();
        dietaryNotes = string.IsNullOrWhiteSpace(dietaryNotes) ? null : dietaryNotes.Trim();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            TempData["RsvpError"] = "Please enter your name.";
            return Redirect(SafeReturn(returnUrl, slug, settings.WeddingId));
        }

        if (reply is not (RsvpStatus.Accepted or RsvpStatus.Declined or RsvpStatus.Maybe))
            reply = RsvpStatus.Accepted;

        _db.WebsiteRsvps.Add(new WebsiteRsvp
        {
            WeddingId = settings.WeddingId,
            FullName = fullName.Length > 150 ? fullName[..150] : fullName,
            Email = email is { Length: > 150 } ? email[..150] : email,
            Reply = reply,
            Message = message is { Length: > 1000 } ? message[..1000] : message,
            DietaryNotes = dietaryNotes is { Length: > 300 } ? dietaryNotes[..300] : dietaryNotes,
            PartySize = 1,
            CreatedAt = DateTime.UtcNow
        });

        var guest = await _db.Guests.FirstOrDefaultAsync(g =>
            g.WeddingId == settings.WeddingId &&
            ((!string.IsNullOrWhiteSpace(email) && g.Email == email) || g.FullName == fullName));

        if (guest is null)
        {
            _db.Guests.Add(new Guest
            {
                WeddingId = settings.WeddingId,
                FullName = fullName.Length > 150 ? fullName[..150] : fullName,
                Email = email is { Length: > 150 } ? email[..150] : email,
                RsvpStatus = reply,
                InvitationSent = true,
                PartySize = 1,
                Side = "Website"
            });
        }
        else
        {
            guest.RsvpStatus = reply;
            if (!string.IsNullOrWhiteSpace(email))
                guest.Email = email.Length > 150 ? email[..150] : email;
        }

        await _db.SaveChangesAsync();

        var wedding = await _db.Weddings.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == settings.WeddingId);
        var coupleName = wedding?.CoupleDisplayName ?? settings.SiteTitle;
        var webId = wedding?.WebId ?? 0;

        await _telegram.SendRsvpAlertAsync(
            coupleName,
            webId,
            slug,
            fullName,
            reply.ToString(),
            email,
            message,
            dietaryNotes);

        TempData["RsvpOk"] = $"Thank you, {fullName} — your RSVP was sent.";
        return Redirect(SafeReturn(returnUrl, slug, settings.WeddingId));
    }

    private IActionResult BackToEdit(int weddingId) =>
        weddingId > 0
            ? RedirectToAction(nameof(Edit), new { weddingId })
            : RedirectToAction(nameof(Index));

    private string SafeReturn(string? returnUrl, string slug, int weddingId)
    {
        // Prefer staying on Preview when testing a draft invite
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            returnUrl.Contains("/Website/Preview", StringComparison.OrdinalIgnoreCase))
        {
            return $"/Website/Preview?weddingId={weddingId}#rsvp";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) &&
            returnUrl.StartsWith("/w/", StringComparison.OrdinalIgnoreCase))
        {
            var pathOnly = returnUrl.Split('?', '#')[0];
            return $"{pathOnly}#rsvp";
        }

        return $"/w/{slug}#rsvp";
    }

    private async Task<WebsiteSettings?> GetOrCreateSettingsAsync(int weddingId)
    {
        var wedding = await _db.Weddings
            .Include(w => w.WebsiteSettings)
            .FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return null;

        if (wedding.WebsiteSettings is null)
        {
            var baseSlug = NormalizeSlug($"{wedding.Partner1Name}-{wedding.Partner2Name}") ?? $"wedding-{wedding.Id}";
            wedding.WebsiteSettings = new WebsiteSettings
            {
                WeddingId = wedding.Id,
                SiteTitle = wedding.CoupleDisplayName,
                Slug = await EnsureUniqueSlugAsync(baseSlug, excludeId: null),
                WelcomeMessage = "We're getting married! Join us for our celebration.",
                ShareDescription = "Join us for our wedding celebration.",
                InviteMessage = "We can't wait to celebrate with you.",
                ThemeLayout = 6,
                EffectStyle = InviteEffects.HeartsSnow,
                ShowMap = true,
                IsPublished = false
            };
            _db.WebsiteSettings.Add(wedding.WebsiteSettings);
            await _db.SaveChangesAsync();
        }

        return wedding.WebsiteSettings;
    }

    private async Task LoadMediaAsync(WebsiteSettings settings)
    {
        await _db.Entry(settings).Collection(s => s.BankQrs).LoadAsync();
        await _db.Entry(settings).Collection(s => s.GalleryPhotos).LoadAsync();
        await _db.Entry(settings).Collection(s => s.CoverPhotos).LoadAsync();
        settings.BankQrs = settings.BankQrs.OrderBy(x => x.SortOrder).ToList();
        settings.GalleryPhotos = settings.GalleryPhotos.OrderBy(x => x.SortOrder).ToList();
        settings.CoverPhotos = settings.CoverPhotos.OrderBy(x => x.SortOrder).ToList();
        await EnsureLegacyCoverAsync(settings);
    }

    private async Task EnsureLegacyCoverAsync(WebsiteSettings settings)
    {
        if (settings.CoverPhotos.Any()) return;
        if (string.IsNullOrWhiteSpace(settings.CoverImageUrl)) return;

        settings.CoverPhotos.Add(new WebsiteCoverPhoto
        {
            WebsiteSettingsId = settings.Id,
            FilePath = settings.CoverImageUrl,
            Label = "Cover 1",
            SortOrder = 1
        });
        await _db.SaveChangesAsync();
        settings.CoverPhotos = settings.CoverPhotos.OrderBy(x => x.SortOrder).ToList();
    }

    private async Task SyncCoverImageUrlAsync(WebsiteSettings settings)
    {
        var first = settings.CoverPhotos
            .Where(x => _db.Entry(x).State != EntityState.Deleted)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.FilePath)
            .FirstOrDefault();
        settings.CoverImageUrl = first;
        await Task.CompletedTask;
    }

    private async Task<string?> EnsureUniqueSlugAsync(string? slug, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(slug))
            slug = $"wedding-{Guid.NewGuid():N}"[..12];

        var candidate = slug;
        var i = 2;
        while (await _db.WebsiteSettings.AnyAsync(s =>
                   s.Slug == candidate && (excludeId == null || s.Id != excludeId)))
        {
            candidate = $"{slug}-{i++}";
        }
        return candidate;
    }

    private async Task<string?> ResolveImageAsync(
        IFormFile? upload,
        string? pastedUrl,
        string? currentUrl,
        string folder)
    {
        if (upload is { Length: > 0 })
        {
            var saved = await SaveImageAsync(upload, folder);
            if (saved is null)
            {
                TempData["Error"] = "Image upload failed (PNG/JPEG/WebP, max ~900KB).";
                return currentUrl;
            }
            if (!string.Equals(currentUrl, saved, StringComparison.OrdinalIgnoreCase))
                DeletePhysical(currentUrl);
            return saved;
        }

        var next = SanitizeImageUrl(pastedUrl);
        if (!string.Equals(currentUrl, next, StringComparison.OrdinalIgnoreCase))
            DeletePhysical(currentUrl);
        return next;
    }

    private static string? SanitizeImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        url = url.Trim();
        if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return null;
        return url.Length > 1000 ? url[..1000] : url;
    }

    private async Task<string?> SaveImageAsync(IFormFile file, string folder)
    {
        if (file.Length <= 0 || file.Length > MaxImageBytes) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not (".png" or ".jpg" or ".jpeg" or ".webp")) return null;
        var contentType = file.ContentType?.ToLowerInvariant() ?? "";
        if (!contentType.StartsWith("image/")) return null;

        var dir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(dir);
        var name = $"{Guid.NewGuid():N}{ext}";
        var physical = Path.Combine(dir, name);
        await using var stream = System.IO.File.Create(physical);
        await file.CopyToAsync(stream);
        return $"/uploads/{folder}/{name}";
    }

    private void DeletePhysical(string? webPath)
    {
        if (string.IsNullOrWhiteSpace(webPath)) return;
        if (!webPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;
        var relative = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physical = Path.Combine(_env.WebRootPath, relative);
        if (System.IO.File.Exists(physical))
            System.IO.File.Delete(physical);
    }

    private static string? NormalizeSlug(string? slug) =>
        string.IsNullOrWhiteSpace(slug) ? null :
        slug.Trim().ToLowerInvariant().Replace(" ", "-");

    private async Task<int> NextWebIdAsync()
    {
        var max = await _db.Weddings.MaxAsync(w => (int?)w.WebId) ?? 2025;
        return Math.Max(max + 1, 2026);
    }

    private string BuildPublicUrl(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return "";
        return $"{Request.Scheme}://{Request.Host}/w/{slug}";
    }
}
