using Microsoft.AspNetCore.Identity;

namespace WeddingManagementSystem.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    public string? DisplayName { get; set; }
}
