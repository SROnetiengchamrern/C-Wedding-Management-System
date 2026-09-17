using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.ViewModels;

public class GiftMoneyViewModel
{
    public WeddingGift Form { get; set; } = new();
    public List<WeddingGift> Gifts { get; set; } = new();
    public string? Search { get; set; }
    public bool IsEditing => Form.Id > 0;

    public int TotalGuests => Gifts.Count;
    public decimal TotalKhr => Gifts.Sum(g => g.AmountKhr);
    public decimal TotalUsd => Gifts.Sum(g => g.AmountUsd);
}
