using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wedding> Weddings => Set<Wedding>();
    public DbSet<WeddingTask> Tasks => Set<WeddingTask>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<StyleDecision> StyleDecisions => Set<StyleDecision>();
    public DbSet<Agreement> Agreements => Set<Agreement>();
    public DbSet<TimelineEvent> TimelineEvents => Set<TimelineEvent>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();
    public DbSet<ScheduleItem> ScheduleItems => Set<ScheduleItem>();
    public DbSet<WebsiteSettings> WebsiteSettings => Set<WebsiteSettings>();
    public DbSet<ImportedRecord> ImportedRecords => Set<ImportedRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wedding>(e =>
        {
            e.Property(x => x.Partner1Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Partner2Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.TotalBudget).HasPrecision(18, 2);
        });

        modelBuilder.Entity<WeddingTask>()
            .HasOne(x => x.Wedding).WithMany(w => w.Tasks).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.HasOne(x => x.Wedding).WithMany(w => w.Payments).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Vendor).WithMany(v => v.Payments).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Document>()
            .HasOne(x => x.Wedding).WithMany(w => w.Documents).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guest>()
            .HasOne(x => x.Wedding).WithMany(w => w.Guests).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vendor>()
            .HasOne(x => x.Wedding).WithMany(w => w.Vendors).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StyleDecision>()
            .HasOne(x => x.Wedding).WithMany(w => w.StyleDecisions).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Agreement>()
            .HasOne(x => x.Wedding).WithMany(w => w.Agreements).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimelineEvent>()
            .HasOne(x => x.Wedding).WithMany(w => w.TimelineEvents).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InboxMessage>()
            .HasOne(x => x.Wedding).WithMany(w => w.InboxMessages).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItem>(e =>
        {
            e.Property(x => x.PricePerGuest).HasPrecision(18, 2);
            e.HasOne(x => x.Wedding).WithMany(w => w.MenuItems).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DeliveryItem>()
            .HasOne(x => x.Wedding).WithMany(w => w.DeliveryItems).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScheduleItem>()
            .HasOne(x => x.Wedding).WithMany(w => w.ScheduleItems).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ImportedRecord>()
            .HasOne(x => x.Wedding).WithMany(w => w.ImportedRecords).HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WebsiteSettings>()
            .HasOne(x => x.Wedding)
            .WithOne(w => w.WebsiteSettings)
            .HasForeignKey<WebsiteSettings>(x => x.WeddingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
