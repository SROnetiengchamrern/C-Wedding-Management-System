using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wedding> Weddings => Set<Wedding>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<WebsiteSettings> WebsiteSettings => Set<WebsiteSettings>();
    public DbSet<WebsiteBankQr> WebsiteBankQrs => Set<WebsiteBankQr>();
    public DbSet<WebsiteGalleryPhoto> WebsiteGalleryPhotos => Set<WebsiteGalleryPhoto>();
    public DbSet<WebsiteRsvp> WebsiteRsvps => Set<WebsiteRsvp>();
    public DbSet<WeddingGift> WeddingGifts => Set<WeddingGift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wedding>(e =>
        {
            e.Property(x => x.Partner1Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Partner2Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.TotalBudget).HasPrecision(18, 2);
            e.HasIndex(x => x.WebId).IsUnique();
        });

        modelBuilder.Entity<Guest>()
            .HasOne(x => x.Wedding).WithMany(w => w.Guests).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeddingGift>(e =>
        {
            e.Property(x => x.AmountKhr).HasPrecision(18, 2);
            e.Property(x => x.AmountUsd).HasPrecision(18, 2);
            e.HasOne(x => x.Wedding).WithMany(w => w.WeddingGifts).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WebsiteSettings>()
            .HasOne(x => x.Wedding)
            .WithOne(w => w.WebsiteSettings)
            .HasForeignKey<WebsiteSettings>(x => x.WeddingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WebsiteBankQr>()
            .HasOne(x => x.WebsiteSettings)
            .WithMany(w => w.BankQrs)
            .HasForeignKey(x => x.WebsiteSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WebsiteGalleryPhoto>()
            .HasOne(x => x.WebsiteSettings)
            .WithMany(w => w.GalleryPhotos)
            .HasForeignKey(x => x.WebsiteSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WebsiteRsvp>()
            .HasOne(x => x.Wedding)
            .WithMany()
            .HasForeignKey(x => x.WeddingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
