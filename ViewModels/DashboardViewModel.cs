namespace WeddingManagementSystem.ViewModels;

public class DashboardViewModel
{
    public int CouplesCount { get; set; }
    public int PublishedSites { get; set; }
    public int DraftSites { get; set; }
    public int GiftGuests { get; set; }
    public decimal GiftKhr { get; set; }
    public decimal GiftUsd { get; set; }
    public int RsvpTotal { get; set; }
    public int RsvpAccepted { get; set; }
    public int RsvpDeclined { get; set; }
    public int RsvpMaybe { get; set; }
    public List<DashboardCoupleRow> Couples { get; set; } = new();
}

public class DashboardCoupleRow
{
    public int WeddingId { get; set; }
    public int WebId { get; set; }
    public string CoupleName { get; set; } = "";
    public DateTime WeddingDate { get; set; }
    public string DateDisplayShort { get; set; } = "";
    public string? VenueName { get; set; }
    public bool IsPublished { get; set; }
    public string? Slug { get; set; }
    public int GiftCount { get; set; }
    public decimal GiftKhr { get; set; }
    public decimal GiftUsd { get; set; }
    public int RsvpCount { get; set; }
    public int DaysLeft { get; set; }
}
