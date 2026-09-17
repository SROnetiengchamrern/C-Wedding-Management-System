using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class Vendor
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(150)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
