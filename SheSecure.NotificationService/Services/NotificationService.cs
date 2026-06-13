using SheSecure.NotificationService.DTOs.Requests;
using SheSecure.NotificationService.DTOs.Responses;
using SheSecure.NotificationService.Entities;
using SheSecure.NotificationService.Interfaces;

namespace SheSecure.NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<NotificationResponseDTO>
            CreateNotificationAsync(
                CreateNotificationDTO dto)
        {
            var notification = new Notification
            {
                EmployeeId = dto.EmployeeId.ToString(),
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type
            };

            var created =
                await _repository.CreateNotificationAsync(
                    notification);

            return new NotificationResponseDTO
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId.ToString(),
                Title = created.Title,
                Message = created.Message,
                Type = created.Type,
                IsRead = created.IsRead,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<List<NotificationResponseDTO>>
            GetAllNotificationsAsync()
        {
            var notifications =
                await _repository.GetAllNotificationsAsync();

            return notifications.Select(x =>
                new NotificationResponseDTO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId.ToString(),
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                }).ToList();
        }

        public async Task<List<NotificationResponseDTO>>
            GetEmployeeNotificationsAsync(
                string employeeId)
        {
            var notifications =
                await _repository.GetEmployeeNotificationsAsync(
                    employeeId);

            return notifications.Select(x =>
                new NotificationResponseDTO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId.ToString(),
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                }).ToList();
        }

        public async Task MarkAsReadAsync(
            int notificationId)
        {
            var notification =
                await _repository.GetByIdAsync(
                    notificationId);

            if (notification == null)
                throw new Exception(
                    "Notification not found");

            notification.IsRead = true;

            await _repository.UpdateNotificationAsync(
                notification);
        }
    }
}