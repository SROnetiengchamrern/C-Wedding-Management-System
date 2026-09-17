using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public enum AgreementStatus
{
    Draft = 0,
    Pending = 1,
    Signed = 2,
    Expired = 3
}

public class Agreement
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public AgreementStatus Status { get; set; } = AgreementStatus.Draft;

    public DateTime? SignedAt { get; set; }
}
