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
    }
}