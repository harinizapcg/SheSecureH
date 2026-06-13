//using SheSecure.Safety_WellnessService.DTOs.Requests;
//using SheSecure.Safety_WellnessService.Entities;
//using SheSecure.Safety_WellnessService.Interfaces;

//namespace SheSecure.Safety_WellnessService.Services
//{
//    public class EmergencyAlertService : IEmergencyAlertService
//    {
//        private readonly IEmergencyAlertRepository _repository;

//        public EmergencyAlertService(
//            IEmergencyAlertRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<EmergencyAlert> CreateAlertAsync(
//            CreateEmergencyAlertDTO dto)
//        {
//            var alert = new EmergencyAlert
//            {
//                EmployeeId = dto.EmployeeId,
//                Location = dto.Location,
//                Description = dto.Description,
//                Severity = dto.Severity,
//                Status = "Active",
//                TriggeredAt = DateTime.UtcNow
//            };

//            return await _repository.CreateAlertAsync(alert);
//        }

//        public async Task<List<EmergencyAlert>> GetAllAlertsAsync()
//        {
//            return await _repository.GetAllAlertsAsync();
//        }

//        public async Task<EmergencyAlert?> GetAlertByIdAsync(
//            int id)
//        {
//            return await _repository.GetByIdAsync(id);
//        }

//        public async Task ResolveAlertAsync(
//            ResolveEmergencyAlertDTO dto)
//        {
//            var alert =
//                await _repository.GetByIdAsync(dto.AlertId);

//            if (alert == null)
//                throw new Exception("Alert not found");

//            alert.Status = "Resolved";
//            alert.ResolvedAt = DateTime.UtcNow;

//            await _repository.UpdateAlertAsync(alert);
//        }
//    }
//}
using System.Text;
using System.Text.Json;
using SheSecure.Safety_WellnessService.DTOs.Requests;
using SheSecure.Safety_WellnessService.Entities;
using SheSecure.Safety_WellnessService.Interfaces;

namespace SheSecure.Safety_WellnessService.Services
{
    public class EmergencyAlertService : IEmergencyAlertService
    {
        private readonly IEmergencyAlertRepository _repository;
        private readonly HttpClient _http;
        private readonly ILogger<EmergencyAlertService> _logger;

        public EmergencyAlertService(
            IEmergencyAlertRepository repository,
            IHttpClientFactory httpFactory,
            ILogger<EmergencyAlertService> logger)
        {
            _repository = repository;
            _http = httpFactory.CreateClient("NotificationService");
            _logger = logger;
        }

        public async Task<EmergencyAlert> CreateAlertAsync(
            CreateEmergencyAlertDTO dto)
        {
            var alert = new EmergencyAlert
            {
                EmployeeId = dto.EmployeeId,
                Location = dto.Location,
                Description = dto.Description,
                Severity = dto.Severity,
                Status = "Active",
                TriggeredAt = DateTime.UtcNow
            };

            var created =
                await _repository.CreateAlertAsync(alert);

            await SendNotificationAsync(
                dto.EmployeeId.ToString(),
                "Emergency SOS Raised",
                "Your SOS alert has been triggered. Help is on the way.",
                "SOS_RAISED");

            return created;
        }

        public async Task<List<EmergencyAlert>>
            GetAllAlertsAsync() =>
            await _repository.GetAllAlertsAsync();

        public async Task<EmergencyAlert?> GetAlertByIdAsync(
            int id) =>
            await _repository.GetByIdAsync(id);

        public async Task ResolveAlertAsync(
            ResolveEmergencyAlertDTO dto)
        {
            var alert =
                await _repository.GetByIdAsync(dto.AlertId);

            if (alert == null)
                throw new Exception("Alert not found");

            alert.Status = "Resolved";
            alert.ResolvedAt = DateTime.UtcNow;

            await _repository.UpdateAlertAsync(alert);

            await SendNotificationAsync(
                alert.EmployeeId.ToString(),
                "SOS Alert Resolved",
                "Your emergency alert has been marked as resolved.",
                "SOS_RESOLVED");
        }

        private async Task SendNotificationAsync(
            string employeeId,
            string title,
            string message,
            string type)
        {
            try
            {
                var payload = JsonSerializer.Serialize(
                    new { employeeId, title, message, type },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase
                    });

                await _http.PostAsync(
                    "api/Notification/create",
                    new StringContent(
                        payload,
                        Encoding.UTF8,
                        "application/json"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send notification [{Type}] " +
                    "for employee {EmployeeId}", type, employeeId);
            }
        }
    }
}