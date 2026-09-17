using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingManagementSystem.Models;

public class MenuItem
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Course { get; set; } = "Main";

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? DietaryNotes { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerGuest { get; set; }
}
