using Microsoft.AspNetCore.Mvc;
using SheSecure.DashboardService.Interfaces;

namespace SheSecure.DashboardService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(
            IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            return Ok(
                await _service.GetStatsAsync());
        }

        [HttpGet("complaints")]
        public async Task<IActionResult>
            GetComplaintAnalytics()
        {
            return Ok(
                await _service
                    .GetComplaintAnalyticsAsync());
        }

        [HttpGet("wellness")]
        public async Task<IActionResult>
            GetWellnessAnalytics()
        {
            return Ok(
                await _service
                    .GetWellnessAnalyticsAsync());
        }

        [HttpGet("emergency")]
        public async Task<IActionResult>
            GetEmergencyAnalytics()
        {
            return Ok(
                await _service
                    .GetEmergencyAnalyticsAsync());
        }

        /// <summary>
        /// Returns a summary dashboard for a single employee:
        /// total complaints, wellness requests, safe reach records,
        /// and unread notifications belonging to that employee.
        /// </summary>
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult>
            GetEmployeeDashboard(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return BadRequest("employeeId is required.");

            return Ok(
                await _service
                    .GetEmployeeDashboardAsync(employeeId));
        }

        /// <summary>
        /// Returns aggregate statistics for admin view:
        /// total complaints, open complaints, pending wellness,
        /// safe reach counts, SOS alerts, and notification stats.
        /// </summary>
        [HttpGet("admin")]
        public async Task<IActionResult>
            GetAdminDashboard()
        {
            return Ok(
                await _service.GetAdminDashboardAsync());
        }
    }
}