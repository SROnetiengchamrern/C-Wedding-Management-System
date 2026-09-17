using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.ViewModels;

public class DashboardViewModel
{
    public Wedding Wedding { get; set; } = null!;

    public int DaysLeft { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal TotalBudget { get; set; }

    public int OverdueTasks { get; set; }
    public int PaymentsOnTrack { get; set; }
    public int TasksThisWeek { get; set; }
    public int PaymentsThisWeek { get; set; }

    public WeddingTask? NextStepTask { get; set; }
    public WeddingTask? UploadContractTask { get; set; }
    public Payment? NextPayment { get; set; }

    public List<WeddingTask> UpcomingDeadlines { get; set; } = new();
    public List<Payment> UpcomingPayments { get; set; } = new();
    public List<Document> RecentDocuments { get; set; } = new();
    public List<StyleDecision> ApprovedStyles { get; set; } = new();

    public int GuestEssentialsDone { get; set; }
    public int GuestEssentialsTotal { get; set; } = 8;
}
