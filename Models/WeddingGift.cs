using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingManagementSystem.Models;

public class WeddingGift
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    [Display(Name = "Guest")]
    public string GuestName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount (KHR)")]
    public decimal AmountKhr { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount (USD)")]
    public decimal AmountUsd { get; set; }

    [MaxLength(200)]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [MaxLength(100)]
    [Display(Name = "Relationship")]
    public string? Relationship { get; set; }

    [Display(Name = "Gift date")]
    public DateTime GiftDate { get; set; } = DateTime.Today;

    [MaxLength(500)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
}
