using SheSecure.Safety_WellnessService.DTOs.Requests;
using SheSecure.Safety_WellnessService.Entities;
using SheSecure.Safety_WellnessService.Interfaces;

namespace SheSecure.Safety_WellnessService.Services
{
    public class EmergencyAlertService : IEmergencyAlertService
    {
        private readonly IEmergencyAlertRepository _repository;

        public EmergencyAlertService(
            IEmergencyAlertRepository repository)
        {
            _repository = repository;
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

            return await _repository.CreateAlertAsync(alert);
        }

        public async Task<List<EmergencyAlert>> GetAllAlertsAsync()
        {
            return await _repository.GetAllAlertsAsync();
        }

        public async Task<EmergencyAlert?> GetAlertByIdAsync(
            int id)
        {
            return await _repository.GetByIdAsync(id);
        }

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
        }
    }
}