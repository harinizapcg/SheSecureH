using Hangfire;
using SheSecure.Safety_WellnessService.DTOs;
using SheSecure.Safety_WellnessService.Entities;
using SheSecure.Safety_WellnessService.Interfaces;
using SheSecure.Safety_WellnessService.Jobs;

namespace SheSecure.Safety_WellnessService.Services
{
    public class SafeReachService : ISafeReachService
    {
        private readonly ISafeReachRepository _repository;

        public SafeReachService(
            ISafeReachRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateAsync(
            CreateSafeReachDTO dto)
        {
            var check = new SafeReachCheck
            {
                EmployeeId = dto.EmployeeId,
                ExpectedArrivalTime = dto.ExpectedArrivalTime,
                IsAcknowledged = false,
                Status = "Pending"
            };

            await _repository.CreateAsync(check);

            // Normalize to UTC
            var expectedUtc =
                dto.ExpectedArrivalTime.Kind == DateTimeKind.Utc
                    ? dto.ExpectedArrivalTime
                    : dto.ExpectedArrivalTime.ToUniversalTime();

            var now = DateTime.UtcNow;

            // Job 1 — reminder at expected arrival time
            var reminderDelay = expectedUtc - now;
            if (reminderDelay > TimeSpan.Zero)
            {
                BackgroundJob.Schedule<SafeReachReminderJob>(
                    job => job.SendReminderAsync(check.Id),
                    reminderDelay);
            }
            else
            {
                BackgroundJob.Enqueue<SafeReachReminderJob>(
                    job => job.SendReminderAsync(check.Id));
            }

            // Job 2 — escalation 30 min after expected arrival
            var escalationDelay =
                expectedUtc - now + TimeSpan.FromMinutes(30);
            if (escalationDelay > TimeSpan.Zero)
            {
                BackgroundJob.Schedule<SafeReachReminderJob>(
                    job => job.CheckAndEscalateAsync(check.Id),
                    escalationDelay);
            }
            else
            {
                BackgroundJob.Schedule<SafeReachReminderJob>(
                    job => job.CheckAndEscalateAsync(check.Id),
                    TimeSpan.FromMinutes(30));
            }
        }

        public async Task AcknowledgeAsync(
            AcknowledgeSafeReachDTO dto)
        {
            var check =
                await _repository.GetByIdAsync(
                    dto.SafeReachId);

            if (check == null)
                throw new Exception(
                    "Safe Reach record not found");

            check.IsAcknowledged = true;
            check.AcknowledgedAt = DateTime.UtcNow;
            check.Status = "Acknowledged";

            await _repository.UpdateAsync(check);
        }

        public async Task EscalateAsync(int id)
        {
            var check =
                await _repository.GetByIdAsync(id);

            if (check == null)
                throw new Exception(
                    "Safe Reach record not found");

            if (check.IsAcknowledged)
                throw new Exception(
                    "Employee already acknowledged");

            check.Status = "Escalated";
            await _repository.UpdateAsync(check);
        }

        public async Task<object> GetAllAsync()
            => await _repository.GetAllAsync();

        public async Task<object> GetByIdAsync(int id)
        {
            var check =
                await _repository.GetByIdAsync(id);

            if (check == null)
                throw new Exception(
                    "Safe Reach record not found");

            return check;
        }
    }
}