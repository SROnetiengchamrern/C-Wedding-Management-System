using Microsoft.EntityFrameworkCore;

namespace WeddingManagementSystem.Data;

public static class WeddingContextExtensions
{
    public static async Task<int?> GetCurrentWeddingIdAsync(this ApplicationDbContext db)
    {
        var wedding = await db.Weddings.AsNoTracking().FirstOrDefaultAsync();
        return wedding?.Id;
    }
}
