using SheSecure.DashboardService.DTOs;

namespace SheSecure.DashboardService.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetStatsAsync();

        Task<List<ComplaintAnalyticsDTO>>
            GetComplaintAnalyticsAsync();

        Task<List<WellnessAnalyticsDTO>>
            GetWellnessAnalyticsAsync();

        Task<List<EmergencyAnalyticsDTO>>
            GetEmergencyAnalyticsAsync();
    }
}