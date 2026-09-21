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

    public ICollection<WeddingTask> Tasks { get; set; } = new List<WeddingTask>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    public ICollection<StyleDecision> StyleDecisions { get; set; } = new List<StyleDecision>();
    public ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    public ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
    public ICollection<InboxMessage> InboxMessages { get; set; } = new List<InboxMessage>();
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
    public ICollection<ScheduleItem> ScheduleItems { get; set; } = new List<ScheduleItem>();
    public ICollection<ImportedRecord> ImportedRecords { get; set; } = new List<ImportedRecord>();
    public ICollection<WeddingGift> WeddingGifts { get; set; } = new List<WeddingGift>();
    public WebsiteSettings? WebsiteSettings { get; set; }

    [NotMapped]
    public string CoupleDisplayName => $"{Partner1Name} & {Partner2Name}";

    [NotMapped]
    public int DaysLeft => Math.Max(0, (WeddingDate.Date - DateTime.Today).Days);
}
