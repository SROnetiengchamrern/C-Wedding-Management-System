using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public enum RsvpStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2,
    Maybe = 3
}

public class Guest
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Side { get; set; }

    public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending;

    public int PartySize { get; set; } = 1;

    public bool InvitationSent { get; set; }
}
