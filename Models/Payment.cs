using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingManagementSystem.Models;

public enum PaymentStatus
{
    Upcoming = 0,
    Due = 1,
    Paid = 2,
    Overdue = 3
}

public class Payment
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Upcoming;
}
