# Wedding Management System

ASP.NET Core MVC + SQL Server wedding planning admin.

## Run

```bash
cd d:\CHAMRERN\WeddingManagementSystem
dotnet run
```

Uses connection string in `appsettings.json`. Migrations and demo seed run on startup.

## Modules

| Page | Route | Features |
|------|--------|----------|
| Overview | `/` | Dashboard stats, next steps, deadlines |
| Tasks | `/Tasks` | List / add / edit / complete |
| Timeline | `/Timeline` | Milestone events |
| Budget | `/Budget` | Payments, mark paid, add payment |
| Wedding Details | `/WeddingDetails` | Couple, venue, style decisions |
| Agreements | `/Agreements` | Contracts, mark signed |
| Documents | `/Documents` | Document registry |
| Inbox | `/Inbox` | Messages + unread badge |
| Menu | `/Menu` | Reception dishes |
| Delivery | `/Delivery` | Vendor deliveries |
| Guests | `/Guests` | Guest list + RSVP |
| Wedding Website | `/Website` | Site settings + preview |
| Day of Schedule | `/Schedule` | Wedding-day timeline |
| Imported | `/Imported` | Import history |
| Settings | `/Settings` | Budget + shared password |
