namespace SheSecure.NotificationService.DTOs.Requests
{
    public class CreateNotificationDTO
    {
        public int EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}