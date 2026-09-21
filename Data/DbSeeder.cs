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

        var vendors = new List<Vendor>
        {
            new() { WeddingId = wedding.Id, Name = "Harvest Table Catering", Category = "Catering", ContactEmail = "hello@harvesttable.example" },
            new() { WeddingId = wedding.Id, Name = "Wildflower Designs", Category = "Florals", ContactEmail = "studio@wildflower.example" },
            new() { WeddingId = wedding.Id, Name = "Rhythm & Groove Entertainment", Category = "Music", ContactEmail = "book@rhythmgroove.example" },
            new() { WeddingId = wedding.Id, Name = "Golden Hour Studios", Category = "Photography", ContactEmail = "hello@goldenhour.example" },
            new() { WeddingId = wedding.Id, Name = "Oakwood Manor", Category = "Venue", ContactEmail = "events@oakwood.example" }
        };
        db.Vendors.AddRange(vendors);
        await db.SaveChangesAsync();

        var catering = vendors.First(v => v.Name == "Harvest Table Catering");
        var florals = vendors.First(v => v.Name == "Wildflower Designs");
        var music = vendors.First(v => v.Name == "Rhythm & Groove Entertainment");
        var venue = vendors.First(v => v.Name == "Oakwood Manor");
        var photo = vendors.First(v => v.Name == "Golden Hour Studios");

        db.Payments.AddRange(
            new Payment
            {
                WeddingId = wedding.Id,
                VendorId = venue.Id,
                Description = "Venue deposit",
                Amount = 5000m,
                DueDate = new DateTime(2025, 11, 1),
                PaidDate = new DateTime(2025, 10, 28),
                Status = PaymentStatus.Paid
            },
            new Payment
            {
                WeddingId = wedding.Id,
                VendorId = photo.Id,
                Description = "Photography retainer",
                Amount = 2000m,
                DueDate = new DateTime(2025, 12, 15),
                PaidDate = new DateTime(2025, 12, 10),
                Status = PaymentStatus.Paid
            },
            new Payment
            {
                WeddingId = wedding.Id,
                VendorId = catering.Id,
                Description = "Deposit — Harvest Table Catering",
                Amount = 3500m,
                DueDate = new DateTime(2026, 1, 25),
                Status = PaymentStatus.Due
            },
            new Payment
            {
                WeddingId = wedding.Id,
                VendorId = florals.Id,
                Description = "Floral deposit — Wildflower Designs",
                Amount = 1000m,
                DueDate = new DateTime(2026, 2, 1),
                Status = PaymentStatus.Upcoming
            },
            new Payment
            {
                WeddingId = wedding.Id,
                VendorId = music.Id,
                Description = "Band deposit — Rhythm & Groove",
                Amount = 800m,
                DueDate = new DateTime(2026, 2, 10),
                Status = PaymentStatus.Upcoming
            }
        );

        var completedTitles = new[]
        {
            "Book venue", "Hire photographer", "Set wedding date", "Create guest list draft",
            "Choose color palette", "Book caterer", "Reserve hotel block", "Send save the dates",
            "Hire DJ / band", "Order invitations", "Select florist", "Taste cake samples",
            "Book hair & makeup", "Reserve rental items", "Plan ceremony outline",
            "Create wedding website", "Open registry", "Book transportation",
            "Schedule engagement photos", "Order wedding bands", "Choose officiant",
            "Plan honeymoon outline", "Book rehearsal dinner restaurant", "Confirm guest room block",
            "Draft day-of timeline"
        };

        foreach (var title in completedTitles)
        {
            db.Tasks.Add(new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = title,
                Status = WeddingTaskStatus.Completed,
                DueDate = DateTime.Today.AddDays(-30),
                Priority = 2
            });
        }

        db.Tasks.AddRange(
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Plan rehearsal dinner",
                Description = "Confirm guest count, menu, and seating for rehearsal dinner.",
                DueDate = new DateTime(2026, 1, 19),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Upload venue contract",
                Description = "Upload the signed Oakwood Manor contract to Documents.",
                DueDate = new DateTime(2026, 1, 15),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Order wedding dress",
                DueDate = new DateTime(2026, 1, 20),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Schedule dress fittings",
                DueDate = new DateTime(2026, 1, 22),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Book rehearsal venue",
                DueDate = new DateTime(2026, 1, 28),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Finalize seating chart draft",
                DueDate = DateTime.Today.AddDays(3),
                Status = WeddingTaskStatus.InProgress,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm catering headcount",
                DueDate = DateTime.Today.AddDays(5),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Review playlist with band",
                DueDate = DateTime.Today.AddDays(6),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Order thank-you cards",
                DueDate = DateTime.Today.AddDays(10),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm florist delivery window",
                DueDate = DateTime.Today.AddDays(12),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Send final RSVP reminder",
                DueDate = DateTime.Today.AddDays(14),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Finalize ceremony readings",
                DueDate = DateTime.Today.AddDays(20),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm vendor meal count",
                DueDate = DateTime.Today.AddDays(25),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Pack emergency kit",
                DueDate = DateTime.Today.AddDays(40),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Print day-of programs",
                DueDate = DateTime.Today.AddDays(45),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Prepare welcome bags",
                DueDate = DateTime.Today.AddDays(50),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm shuttle schedule",
                DueDate = DateTime.Today.AddDays(55),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Final walkthrough with venue",
                DueDate = DateTime.Today.AddDays(60),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Create tip envelopes",
                DueDate = DateTime.Today.AddDays(70),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm photographer shot list",
                DueDate = DateTime.Today.AddDays(75),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Send final headcount to caterer",
                DueDate = DateTime.Today.AddDays(80),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Confirm cake delivery",
                DueDate = DateTime.Today.AddDays(85),
                Status = WeddingTaskStatus.Pending,
                Priority = 2
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Prepare vows",
                DueDate = DateTime.Today.AddDays(90),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Final fittings",
                DueDate = DateTime.Today.AddDays(100),
                Status = WeddingTaskStatus.Pending,
                Priority = 1
            },
            new WeddingTask
            {
                WeddingId = wedding.Id,
                Title = "Pack honeymoon bags",
                DueDate = DateTime.Today.AddDays(110),
                Status = WeddingTaskStatus.Pending,
                Priority = 3
            }
        );

        db.Documents.AddRange(
            new Document
            {
                WeddingId = wedding.Id,
                FileName = "Oakwood Manor Contract.pdf",
                Category = "Contract",
                UploadedAt = DateTime.UtcNow.AddDays(-14)
            },
            new Document
            {
                WeddingId = wedding.Id,
                FileName = "Venue Deposit Receipt.pdf",
                Category = "Receipt",
                UploadedAt = DateTime.UtcNow.AddDays(-12)
            },
            new Document
            {
                WeddingId = wedding.Id,
                FileName = "Golden Hour Studios Contract.pdf",
                Category = "Contract",
                UploadedAt = DateTime.UtcNow.AddDays(-8)
            }
        );

        db.StyleDecisions.AddRange(
            new StyleDecision { WeddingId = wedding.Id, Category = "Flowers", Decision = "Garden roses & eucalyptus", IsApproved = true },
            new StyleDecision { WeddingId = wedding.Id, Category = "Decor", Decision = "Soft gold & ivory tablescapes", IsApproved = true },
            new StyleDecision { WeddingId = wedding.Id, Category = "Table Settings", Decision = "Gold rim chargers", IsApproved = true },
            new StyleDecision { WeddingId = wedding.Id, Category = "Attire", Decision = "Classic ivory & navy", IsApproved = true }
        );

        db.Agreements.AddRange(
            new Agreement { WeddingId = wedding.Id, Title = "Venue agreement", Status = AgreementStatus.Signed, SignedAt = DateTime.UtcNow.AddDays(-40) },
            new Agreement { WeddingId = wedding.Id, Title = "Photography agreement", Status = AgreementStatus.Signed, SignedAt = DateTime.UtcNow.AddDays(-30) },
            new Agreement { WeddingId = wedding.Id, Title = "Catering agreement", Status = AgreementStatus.Pending }
        );

        db.Guests.AddRange(
            new Guest { WeddingId = wedding.Id, FullName = "Sarah Chen", Email = "sarah@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 2 },
            new Guest { WeddingId = wedding.Id, FullName = "Michael Torres", Email = "michael@example.com", Side = "Groom", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 1 },
            new Guest { WeddingId = wedding.Id, FullName = "Olivia Park", Email = "olivia@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Pending, InvitationSent = true, PartySize = 2 },
            new Guest { WeddingId = wedding.Id, FullName = "Daniel Kim", Email = "daniel@example.com", Side = "Groom", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 2 },
            new Guest { WeddingId = wedding.Id, FullName = "Ava Martinez", Email = "ava@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Maybe, InvitationSent = true, PartySize = 1 },
            new Guest { WeddingId = wedding.Id, FullName = "Noah Williams", Email = "noah@example.com", Side = "Groom", RsvpStatus = RsvpStatus.Declined, InvitationSent = true, PartySize = 1 },
            new Guest { WeddingId = wedding.Id, FullName = "Sophia Nguyen", Email = "sophia@example.com", Side = "Bride", RsvpStatus = RsvpStatus.Accepted, InvitationSent = true, PartySize = 2 },
            new Guest { WeddingId = wedding.Id, FullName = "Liam Brooks", Email = "liam@example.com", Side = "Groom", RsvpStatus = RsvpStatus.Pending, InvitationSent = false, PartySize = 1 }
        );

        await db.SaveChangesAsync();
        await SeedModulesAsync(db, wedding.Id);
    }

    public static async Task SeedModulesAsync(ApplicationDbContext db, int? weddingId = null)
    {
        if (weddingId is null)
        {
            var wedding = await db.Weddings.AsNoTracking().FirstOrDefaultAsync();
            if (wedding is null) return;
            weddingId = wedding.Id;
        }
        var id = weddingId.Value;

        if (!db.TimelineEvents.Any())
        {
            db.TimelineEvents.AddRange(
                new TimelineEvent { WeddingId = id, Title = "Engagement party", EventDate = new DateTime(2025, 10, 12), Category = "Celebration", Description = "Family gathering at Oakwood." },
                new TimelineEvent { WeddingId = id, Title = "Save the dates sent", EventDate = new DateTime(2025, 11, 1), Category = "Guests" },
                new TimelineEvent { WeddingId = id, Title = "Venue walkthrough", EventDate = new DateTime(2026, 3, 15), Category = "Venue" },
                new TimelineEvent { WeddingId = id, Title = "Wedding day", EventDate = new DateTime(2026, 9, 22), Category = "Ceremony", Description = "Oakwood Manor, Sonoma County" }
            );
        }

        if (!db.InboxMessages.Any())
        {
            db.InboxMessages.AddRange(
                new InboxMessage { WeddingId = id, FromName = "Oakwood Manor", FromEmail = "events@oakwood.example", Subject = "Final venue checklist", Body = "Please review the attached day-of checklist for Sep 22.", IsRead = false, ReceivedAt = DateTime.UtcNow.AddDays(-1) },
                new InboxMessage { WeddingId = id, FromName = "Harvest Table Catering", FromEmail = "hello@harvesttable.example", Subject = "Tasting appointment confirmation", Body = "Your tasting is confirmed for next Thursday at 4pm.", IsRead = false, ReceivedAt = DateTime.UtcNow.AddDays(-2) },
                new InboxMessage { WeddingId = id, FromName = "Wildflower Designs", FromEmail = "studio@wildflower.example", Subject = "Moodboard draft ready", Body = "We've shared the floral moodboard for your review.", IsRead = false, ReceivedAt = DateTime.UtcNow.AddHours(-8) }
            );
        }

        if (!db.MenuItems.Any())
        {
            db.MenuItems.AddRange(
                new MenuItem { WeddingId = id, Name = "Heirloom tomato salad", Course = "Starter", DietaryNotes = "Vegetarian", PricePerGuest = 12 },
                new MenuItem { WeddingId = id, Name = "Herb-roasted chicken", Course = "Main", Description = "With seasonal vegetables", PricePerGuest = 38 },
                new MenuItem { WeddingId = id, Name = "Seared salmon", Course = "Main", DietaryNotes = "Gluten-free option", PricePerGuest = 42 },
                new MenuItem { WeddingId = id, Name = "Lemon olive oil cake", Course = "Dessert", DietaryNotes = "Contains nuts", PricePerGuest = 14 }
            );
        }

        if (!db.DeliveryItems.Any())
        {
            db.DeliveryItems.AddRange(
                new DeliveryItem { WeddingId = id, ItemName = "Floral centerpieces", VendorName = "Wildflower Designs", DeliveryDate = new DateTime(2026, 9, 22), DropOffLocation = "Reception hall", Status = DeliveryStatus.Scheduled },
                new DeliveryItem { WeddingId = id, ItemName = "Cake", VendorName = "Sweet Crumb Bakery", DeliveryDate = new DateTime(2026, 9, 22), DropOffLocation = "Kitchen", Status = DeliveryStatus.Scheduled },
                new DeliveryItem { WeddingId = id, ItemName = "Rentals — linens & chargers", VendorName = "Sonoma Event Rentals", DeliveryDate = new DateTime(2026, 9, 21), DropOffLocation = "Loading dock", Status = DeliveryStatus.Scheduled }
            );
        }

        if (!db.ScheduleItems.Any())
        {
            db.ScheduleItems.AddRange(
                new ScheduleItem { WeddingId = id, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(14, 0, 0), Title = "Getting ready photos", Location = "Bridal suite", SortOrder = 1 },
                new ScheduleItem { WeddingId = id, StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(15, 45, 0), Title = "Ceremony", Location = "Garden terrace", SortOrder = 2 },
                new ScheduleItem { WeddingId = id, StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 0, 0), Title = "Cocktail hour", Location = "Courtyard", SortOrder = 3 },
                new ScheduleItem { WeddingId = id, StartTime = new TimeSpan(17, 15, 0), EndTime = new TimeSpan(21, 0, 0), Title = "Reception & dinner", Location = "Ballroom", SortOrder = 4 },
                new ScheduleItem { WeddingId = id, StartTime = new TimeSpan(21, 0, 0), EndTime = new TimeSpan(23, 0, 0), Title = "Dancing", Location = "Ballroom", SortOrder = 5 }
            );
        }

        if (!db.WebsiteSettings.Any())
        {
            db.WebsiteSettings.Add(new WebsiteSettings
            {
                WeddingId = id,
                SiteTitle = "Emma & James",
                Slug = "emma-james",
                WelcomeMessage = "We're tying the knot at Oakwood Manor. We can't wait to celebrate with you.",
                RsvpUrl = "/Guests",
                IsPublished = true
            });
        }

        if (!db.ImportedRecords.Any())
        {
            db.ImportedRecords.AddRange(
                new ImportedRecord { WeddingId = id, Source = "Google Sheets", Name = "Guest list v2.csv", RecordType = "Guest", Status = "Imported", ImportedAt = DateTime.UtcNow.AddDays(-10) },
                new ImportedRecord { WeddingId = id, Source = "The Knot", Name = "Vendor contacts", RecordType = "Vendor", Status = "Imported", ImportedAt = DateTime.UtcNow.AddDays(-5) }
            );
        }

        if (!db.WeddingGifts.Any())
        {
            db.WeddingGifts.AddRange(CreateSampleGifts(id));
        }
        else if (db.WeddingGifts.Count() < 50)
        {
            var existing = db.WeddingGifts.Count();
            var needed = CreateSampleGifts(id).Skip(existing).Take(50 - existing);
            db.WeddingGifts.AddRange(needed);
        }

        await db.SaveChangesAsync();
        await SeedSampleCouplesAsync(db);
    }

    /// <summary>Extra sample couples for multi-wedding website list.</summary>
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

            var wedding = new Wedding
            {
                Partner1Name = s.P1,
                Partner2Name = s.P2,
                WeddingDate = s.Date,
                VenueName = s.Venue,
                VenueLocation = s.Loc,
                TotalBudget = 25000m,
                WebId = (await db.Weddings.MaxAsync(w => (int?)w.WebId) ?? 2025) + 1
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

        // Ensure every existing wedding has website settings
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
