using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public enum DeliveryStatus
{
    Scheduled = 0,
    InTransit = 1,
    Delivered = 2,
    Delayed = 3
}

public class DeliveryItem
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? VendorName { get; set; }

    public DateTime? DeliveryDate { get; set; }

    [MaxLength(200)]
    public string? DropOffLocation { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
