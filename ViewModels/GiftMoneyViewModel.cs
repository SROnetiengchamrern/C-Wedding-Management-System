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
    public const decimal UsdToKhrRate = 4000m;

    public int WeddingId { get; set; }
    public int WebId { get; set; }
    public string CoupleName { get; set; } = "";
    public DateTime WeddingDate { get; set; }
    public string? VenueName { get; set; }
    public int GiftCount { get; set; }
    public decimal TotalKhr { get; set; }
    public decimal TotalUsd { get; set; }

    /// <summary>All gifts as KHR using $1 = 4000 ៛.</summary>
    public decimal CombinedKhr => TotalKhr + (TotalUsd * UsdToKhrRate);

    /// <summary>All gifts as USD using $1 = 4000 ៛.</summary>
    public decimal CombinedUsd => TotalUsd + (TotalKhr / UsdToKhrRate);
}

public class GiftTotalsPageViewModel
{
    public List<GiftPartnerListItem> Partners { get; set; } = new();
    public int SelectedWebId { get; set; }
    public GiftPartnerListItem? Selected { get; set; }
    public decimal ExchangeRate => GiftPartnerListItem.UsdToKhrRate;
}
