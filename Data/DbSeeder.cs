using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (db.Weddings.Any())
            return;

        var wedding = new Wedding
        {
            Partner1Name = "Emma",
            Partner2Name = "James",
            WeddingDate = new DateTime(2026, 9, 22),
            VenueName = "Oakwood Manor",
            VenueLocation = "Sonoma County",
            TotalBudget = 44950m,
            WebId = 2026
        };

        db.Weddings.Add(wedding);
        await db.SaveChangesAsync();

        db.Guests.AddRange(
            new Guest { WeddingId = wedding.Id, FullName = "Sarah Chen", Email = "sarah@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 2 },
            new Guest { WeddingId = wedding.Id, FullName = "Michael Torres", Email = "michael@example.com", Side = "Groom", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 1 },
            new Guest { WeddingId = wedding.Id, FullName = "Olivia Park", Email = "olivia@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Pending, InvitationSent = true, PartySize = 2 }
        );

        db.WebsiteSettings.Add(new WebsiteSettings
        {
            WeddingId = wedding.Id,
            SiteTitle = "Emma & James",
            Slug = "emma-james",
            WelcomeMessage = "We're tying the knot at Oakwood Manor. We can't wait to celebrate with you.",
            ShareDescription = "Join us for our wedding celebration.",
            InviteMessage = "We can't wait to celebrate with you.",
            ThemeLayout = 6,
            ShowMap = true,
            IsPublished = true
        });

        db.WeddingGifts.AddRange(CreateSampleGifts(wedding.Id));
        await db.SaveChangesAsync();
        await SeedSampleCouplesAsync(db);
    }

    public static async Task SeedModulesAsync(ApplicationDbContext db, int? weddingId = null)
    {
        // Keep for Program.cs compatibility — ensure website settings + sample couples exist.
        var wedding = weddingId is int id
            ? await db.Weddings.Include(w => w.WebsiteSettings).FirstOrDefaultAsync(w => w.Id == id)
            : await db.Weddings.Include(w => w.WebsiteSettings).OrderBy(w => w.Id).FirstOrDefaultAsync();

        if (wedding is null) return;

        if (wedding.WebsiteSettings is null)
        {
            db.WebsiteSettings.Add(new WebsiteSettings
            {
                WeddingId = wedding.Id,
                SiteTitle = wedding.CoupleDisplayName,
                Slug = "emma-james",
                WelcomeMessage = "We're tying the knot. We can't wait to celebrate with you.",
                ThemeLayout = 6,
                ShowMap = true,
                IsPublished = true
            });
            await db.SaveChangesAsync();
        }

        if (!db.WeddingGifts.Any(g => g.WeddingId == wedding.Id))
        {
            db.WeddingGifts.AddRange(CreateSampleGifts(wedding.Id));
            await db.SaveChangesAsync();
        }

        await SeedSampleCouplesAsync(db);
    }

    public static async Task SeedSampleCouplesAsync(ApplicationDbContext db)
    {
        var samples = new[]
        {
            new { P1 = "Thona", P2 = "Jenna", Date = new DateTime(2026, 11, 15), Venue = "Riverside Garden", Loc = "Phnom Penh", Slug = "thona-jenna", Theme = 2, Pub = true },
            new { P1 = "John", P2 = "Marr", Date = new DateTime(2027, 2, 14), Venue = "Skyline Ballroom", Loc = "Siem Reap", Slug = "john-marr", Theme = 3, Pub = true },
            new { P1 = "Sokha", P2 = "Dara", Date = new DateTime(2026, 12, 5), Venue = "Angkor Pavilion", Loc = "Siem Reap", Slug = "sokha-dara", Theme = 1, Pub = false },
            new { P1 = "Alex", P2 = "Jordan", Date = new DateTime(2027, 4, 20), Venue = "Coastal Manor", Loc = "Kampot", Slug = "alex-jordan", Theme = 5, Pub = true }
        };

        foreach (var s in samples)
        {
            var exists = await db.Weddings.AnyAsync(w =>
                w.Partner1Name == s.P1 && w.Partner2Name == s.P2);
            if (exists) continue;

            var nextWebId = (await db.Weddings.MaxAsync(w => (int?)w.WebId) ?? 2025) + 1;
            var wedding = new Wedding
            {
                Partner1Name = s.P1,
                Partner2Name = s.P2,
                WeddingDate = s.Date,
                VenueName = s.Venue,
                VenueLocation = s.Loc,
                TotalBudget = 25000m,
                WebId = Math.Max(nextWebId, 2026)
            };
            db.Weddings.Add(wedding);
            await db.SaveChangesAsync();

            var slugTaken = await db.WebsiteSettings.AnyAsync(x => x.Slug == s.Slug);
            db.WebsiteSettings.Add(new WebsiteSettings
            {
                WeddingId = wedding.Id,
                SiteTitle = $"{s.P1} & {s.P2}",
                Slug = slugTaken ? $"{s.Slug}-{wedding.Id}" : s.Slug,
                WelcomeMessage = $"Welcome to {s.P1} & {s.P2}'s wedding celebration.",
                ShareDescription = $"You're invited to celebrate with {s.P1} & {s.P2}.",
                InviteMessage = "We can't wait to celebrate with you.",
                ThemeLayout = s.Theme,
                ShowMap = true,
                MapSearch = s.Loc,
                IsPublished = s.Pub
            });
            await db.SaveChangesAsync();
        }

        var missing = await db.Weddings
            .Include(w => w.WebsiteSettings)
            .Where(w => w.WebsiteSettings == null)
            .ToListAsync();
        foreach (var w in missing)
        {
            var baseSlug = $"{w.Partner1Name}-{w.Partner2Name}".Trim().ToLowerInvariant().Replace(" ", "-");
            var slug = baseSlug;
            var i = 2;
            while (await db.WebsiteSettings.AnyAsync(x => x.Slug == slug))
                slug = $"{baseSlug}-{i++}";

            db.WebsiteSettings.Add(new WebsiteSettings
            {
                WeddingId = w.Id,
                SiteTitle = w.CoupleDisplayName,
                Slug = slug,
                WelcomeMessage = "We're getting married! Join us for our celebration.",
                ThemeLayout = 6,
                ShowMap = true,
                IsPublished = false
            });
        }
        if (missing.Count > 0)
            await db.SaveChangesAsync();
    }

    public static async Task SeedAdminUserAsync(IServiceProvider services)
    {
        var users = services.GetRequiredService<UserManager<AppUser>>();
        const string email = "admin@wedding.local";
        const string password = "Admin123!";

        var existing = await users.FindByEmailAsync(email);
        if (existing is not null) return;

        var admin = new AppUser
        {
            UserName = email,
            Email = email,
            DisplayName = "Wedding Admin",
            EmailConfirmed = true
        };
        await users.CreateAsync(admin, password);
    }

    private static List<WeddingGift> CreateSampleGifts(int weddingId)
    {
        var names = new[]
        {
            "Sokha", "Vanna", "Rattana", "Dara", "Maly", "Pisey", "Chantra", "Sovann",
            "Sreyneang", "Bopha", "Chenda", "Nary", "Phalla", "Sopheak", "Veasna", "Kunthea",
            "Arun", "Bora", "Chhay", "Davith", "Eang", "Fiona Chen", "Gunnar", "Hanna Lee",
            "សុខា", "វណ្ណា", "រតនា", "ដារ៉ា", "ម៉ាលី", "ពិសី", "ចំរើន", "សុវណ្ណារិទ្ធ",
            "James Park", "Emily Tran", "Michael Sok", "Lisa Nguyen", "David Kim", "Anna Meas",
            "Rithy", "Sreyleak", "Pich", "Thida", "Vuthy", "Sreymom", "Chanthou", "Makara",
            "Navy", "Sopheap", "Kosal", "Sreypov", "Tola", "Sreymao", "ចន្ទ្រា", "បុប្ផា"
        };

        var addresses = new[]
        {
            "Phnom Penh", "Siem Reap", "Battambang", "Kampot", "Kandal", "Takeo", "Sihanoukville",
            "Kampong Cham", "Prey Veng", "ភ្នំពេញ", "សៀមរាប", "Koh Keo", "Sambour", "Kratie",
            "Pursat", "Oddar Meanchey"
        };

        var relationships = new[]
        {
            "Family", "Friend", "Colleague", "Neighbor", "Cousin", "Uncle", "Aunt", "Other",
            "គ្រួសារ", "មិត្តភក្តិ", "មិត្តរួមការងារ"
        };

        var notes = new[] { "", "", "Table 1", "Table 2", "Table 3", "VIP", "Cash", "Transfer", "Khmer + English OK", "" };
        var khrAmounts = new[] { 0m, 20000m, 50000m, 55000m, 100000m, 150000m, 200000m, 250000m, 300000m };
        var usdAmounts = new[] { 0m, 10m, 20m, 25m, 50m, 75m, 100m, 150m, 200m };

        var list = new List<WeddingGift>(50);
        for (var i = 0; i < 50; i++)
        {
            var useKhr = i % 3 != 1;
            var useUsd = i % 3 != 0;
            list.Add(new WeddingGift
            {
                WeddingId = weddingId,
                GuestName = names[i % names.Length] + (i >= names.Length ? $" {i + 1}" : ""),
                AmountKhr = useKhr ? khrAmounts[i % khrAmounts.Length] : 0,
                AmountUsd = useUsd ? usdAmounts[(i + 2) % usdAmounts.Length] : 0,
                Address = addresses[i % addresses.Length],
                Relationship = relationships[i % relationships.Length],
                GiftDate = DateTime.Today.AddDays(-(i % 14)),
                Notes = notes[i % notes.Length]
            });
        }

        return list;
    }
}
