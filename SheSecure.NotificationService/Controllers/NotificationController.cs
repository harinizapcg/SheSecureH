using Microsoft.AspNetCore.Mvc;
using SheSecure.NotificationService.DTOs.Requests;
using SheSecure.NotificationService.Interfaces;

namespace SheSecure.NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(
            INotificationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(
            CreateNotificationDTO dto)
        {
            var result =
                await _service.CreateNotificationAsync(dto);

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(
                await _service.GetAllNotificationsAsync());
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult>
            GetEmployeeNotifications(
                string employeeId)
        {
            return Ok(
                await _service
                    .GetEmployeeNotificationsAsync(
                        employeeId));
        }

        [HttpPut("read/{id}")]
        public async Task<IActionResult>
            MarkAsRead(int id)
        {
            await _service.MarkAsReadAsync(id);

            return Ok(new
            {
                Message =
                    "Notification marked as read"
            });
        }
    }
}