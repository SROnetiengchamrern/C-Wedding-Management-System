using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingManagementSystem.Models;

public class Wedding
{
    public int Id { get; set; }

    /// <summary>Public website id, starting at 2026 and increasing by 1.</summary>
    public int WebId { get; set; }

    [Required, MaxLength(100)]
    public string Partner1Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Partner2Name { get; set; } = string.Empty;

    [Required]
    public DateTime WeddingDate { get; set; }

    /// <summary>Optional end date when the celebration spans multiple days.</summary>
    public DateTime? WeddingDateEnd { get; set; }

    [MaxLength(200)]
    public string? VenueName { get; set; }

    [MaxLength(200)]
    public string? VenueLocation { get; set; }

    [MaxLength(500)]
    public string? CeremonyNotes { get; set; }

    [MaxLength(100)]
    public string? GuestPassword { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalBudget { get; set; }

    public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    public ICollection<WeddingGift> WeddingGifts { get; set; } = new List<WeddingGift>();
    public WebsiteSettings? WebsiteSettings { get; set; }

    [NotMapped]
    public string CoupleDisplayName => $"{Partner1Name} & {Partner2Name}";

    [NotMapped]
    public int DaysLeft => Math.Max(0, (WeddingDate.Date - DateTime.Today).Days);

    [NotMapped]
    public bool HasDateRange =>
        WeddingDateEnd is DateTime end && end.Date > WeddingDate.Date;

    [NotMapped]
    public string DateDisplayLong => FormatDateLong(WeddingDate, WeddingDateEnd);

    [NotMapped]
    public string DateDisplayShort => FormatDateShort(WeddingDate, WeddingDateEnd);

    public static string FormatDateLong(DateTime start, DateTime? end)
    {
        if (end is null || end.Value.Date <= start.Date)
            return start.ToString("MMMM d, yyyy");
        var e = end.Value;
        if (start.Year == e.Year && start.Month == e.Month)
            return $"{start:MMMM d} – {e:d, yyyy}";
        if (start.Year == e.Year)
            return $"{start:MMMM d} – {e:MMMM d, yyyy}";
        return $"{start:MMMM d, yyyy} – {e:MMMM d, yyyy}";
    }

    public static string FormatDateShort(DateTime start, DateTime? end)
    {
        if (end is null || end.Value.Date <= start.Date)
            return start.ToString("MMM d, yyyy");
        var e = end.Value;
        if (start.Year == e.Year && start.Month == e.Month)
            return $"{start:MMM d} – {e:d, yyyy}";
        if (start.Year == e.Year)
            return $"{start:MMM d} – {e:MMM d, yyyy}";
        return $"{start:MMM d, yyyy} – {e:MMM d, yyyy}";
    }
}
