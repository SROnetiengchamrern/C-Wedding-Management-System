using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.ViewModels;

public class GiftMoneyViewModel
{
    public int WeddingId { get; set; }
    public int WebId { get; set; }
    public string CoupleName { get; set; } = "";
    public WeddingGift Form { get; set; } = new();
    public List<WeddingGift> Gifts { get; set; } = new();
    public string? Search { get; set; }
    public bool IsEditing => Form.Id > 0;

    public int TotalGuests => Gifts.Count;
    public decimal TotalKhr => Gifts.Sum(g => g.AmountKhr);
    public decimal TotalUsd => Gifts.Sum(g => g.AmountUsd);
}

public class GiftPartnerListItem
{
    public int WeddingId { get; set; }
    public int WebId { get; set; }
    public string CoupleName { get; set; } = "";
    public DateTime WeddingDate { get; set; }
    public string? VenueName { get; set; }
    public int GiftCount { get; set; }
    public decimal TotalKhr { get; set; }
    public decimal TotalUsd { get; set; }
}
