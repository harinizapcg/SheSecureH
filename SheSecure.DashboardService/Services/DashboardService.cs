using SheSecure.DashboardService.DTOs;
using SheSecure.DashboardService.Interfaces;
using System.Net.Http.Json;

namespace SheSecure.DashboardService.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DashboardService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<DashboardStatsDTO> GetStatsAsync()
        {
            var client = _httpClientFactory.CreateClient();

            int totalComplaints = 0;
            int openComplaints = 0;
            int resolvedComplaints = 0;
            int wellnessRequests = 0;
            int activeEmergencies = 0;
            int notificationsSent = 0;

            // ── Complaints ────────────────────────────────────────────────
            try
            {
                var complaints = await client
                    .GetFromJsonAsync<List<ComplaintItemDTO>>(
                        $"{ComplaintBase()}/api/Complaint/all");

                if (complaints != null)
                {
                    totalComplaints = complaints.Count;
                    openComplaints = complaints.Count(
                        x => x.Status != "Resolved" && x.Status != "Closed");
                    resolvedComplaints = complaints.Count(
                        x => x.Status == "Resolved" || x.Status == "Closed");
                }
            }
            catch { /* service unavailable — leave as 0 */ }

            // ── Wellness Requests ─────────────────────────────────────────
            try
            {
                var wellness = await client
                    .GetFromJsonAsync<List<WellnessItemDTO>>(
                        $"{WellnessBase()}/api/WellnessRequest/all");

                if (wellness != null)
                    wellnessRequests = wellness.Count;
            }
            catch { }

            // ── Emergency Alerts ──────────────────────────────────────────
            try
            {
                var emergencies = await client
                    .GetFromJsonAsync<List<EmergencyItemDTO>>(
                        $"{WellnessBase()}/api/EmergencyAlert/all");

                if (emergencies != null)
                    activeEmergencies = emergencies.Count(
                        x => x.Status == "Active");
            }
            catch { }

            // ── Notifications ─────────────────────────────────────────────
            try
            {
                var notifications = await client
                    .GetFromJsonAsync<List<NotificationItemDTO>>(
                        $"{NotificationBase()}/api/Notification/all");

                if (notifications != null)
                    notificationsSent = notifications.Count;
            }
            catch { }

            return new DashboardStatsDTO
            {
                TotalComplaints = totalComplaints,
                OpenComplaints = openComplaints,
                ResolvedComplaints = resolvedComplaints,
                WellnessRequests = wellnessRequests,
                ActiveEmergencyAlerts = activeEmergencies,
                NotificationsSent = notificationsSent
            };
        }

        public async Task<List<ComplaintAnalyticsDTO>>
            GetComplaintAnalyticsAsync()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var complaints = await client
                    .GetFromJsonAsync<List<ComplaintItemDTO>>(
                        $"{ComplaintBase()}/api/Complaint/all");

                if (complaints == null || !complaints.Any())
                    return new List<ComplaintAnalyticsDTO>();

                // Group by Category
                return complaints
                    .GroupBy(x => x.Category ?? "Uncategorized")
                    .Select(g => new ComplaintAnalyticsDTO
                    {
                        Category = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }
            catch
            {
                return new List<ComplaintAnalyticsDTO>();
            }
        }

        public async Task<List<WellnessAnalyticsDTO>>
            GetWellnessAnalyticsAsync()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var wellness = await client
                    .GetFromJsonAsync<List<WellnessItemDTO>>(
                        $"{WellnessBase()}/api/WellnessRequest/all");

                if (wellness == null || !wellness.Any())
                    return new List<WellnessAnalyticsDTO>();

                // Group by RequestType
                return wellness
                    .GroupBy(x => x.RequestType ?? "General")
                    .Select(g => new WellnessAnalyticsDTO
                    {
                        RequestType = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }
            catch
            {
                return new List<WellnessAnalyticsDTO>();
            }
        }

        public async Task<List<EmergencyAnalyticsDTO>>
            GetEmergencyAnalyticsAsync()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var emergencies = await client
                    .GetFromJsonAsync<List<EmergencyItemDTO>>(
                        $"{WellnessBase()}/api/EmergencyAlert/all");

                if (emergencies == null || !emergencies.Any())
                    return new List<EmergencyAnalyticsDTO>();

                // Group by Status
                return emergencies
                    .GroupBy(x => x.Status ?? "Unknown")
                    .Select(g => new EmergencyAnalyticsDTO
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }
            catch
            {
                return new List<EmergencyAnalyticsDTO>();
            }
        }

        // ── Base URLs from appsettings ────────────────────────────────────
        private string ComplaintBase() =>
            _configuration["ServiceUrls:ComplaintService"]
                ?? "https://localhost:7032";

        private string WellnessBase() =>
            _configuration["ServiceUrls:WellnessService"]
                ?? "https://localhost:7044";

        private string NotificationBase() =>
            _configuration["ServiceUrls:NotificationService"]
                ?? "https://localhost:7179";
    }

    // ── Internal DTOs for deserializing other services' responses ────────
    // These are lightweight — only the fields Dashboard needs

    internal class ComplaintItemDTO
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
    }

    internal class WellnessItemDTO
    {
        public int Id { get; set; }
        public string? RequestType { get; set; }
        public string? Status { get; set; }
    }

    internal class EmergencyItemDTO
    {
        public int Id { get; set; }
        public string? Status { get; set; }
    }

    internal class NotificationItemDTO
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
    }
}