using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheSecure.Safety_WellnessService.DTOs;
using SheSecure.Safety_WellnessService.Models;
using SheSecure.Safety_WellnessService.Data;





namespace SheSecure.Safety_WellnessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoodController : ControllerBase
    {
        private readonly WellnessDbContext _context;

        public MoodController(WellnessDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateMoodLogDTO dto)
        {
            var mood = new MoodLog
            {
                EmployeeId = dto.EmployeeId,
                MoodLevel = dto.MoodLevel,
                StressLevel = dto.StressLevel,
                Remarks = dto.Remarks
            };

            _context.MoodLogs.Add(mood);

            await _context.SaveChangesAsync();

            return Ok(mood);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.MoodLogs.ToListAsync());
        }
        [HttpGet("burnout-risk/{employeeId}")]
        public async Task<IActionResult> GetBurnoutRisk(string employeeId)
        {
            var logs = await _context.MoodLogs
                .Where(x => x.EmployeeId == employeeId)
                .ToListAsync();

            if (!logs.Any())
                return NotFound("No mood logs found for this employee.");

            var avgMood = logs.Average(x => x.MoodLevel);
            var avgStress = logs.Average(x => x.StressLevel);

            string riskLevel;

            if (avgMood <= 2 && avgStress >= 4)
                riskLevel = "High";
            else if (avgMood <= 3 && avgStress >= 3)
                riskLevel = "Medium";
            else
                riskLevel = "Low";

            var result = new BurnoutRiskDTO
            {
                EmployeeId = employeeId,
                AverageMood = avgMood,
                AverageStress = avgStress,
                RiskLevel = riskLevel
            };

            return Ok(result);
        }
        [HttpGet("trend/{employeeId}")]
        public async Task<IActionResult> GetMoodTrend(string employeeId)
        {
            var logs = await _context.MoodLogs
                .Where(x => x.EmployeeId == employeeId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new MoodTrendDTO
                {
                    Date = x.CreatedAt,
                    MoodLevel = x.MoodLevel,
                    StressLevel = x.StressLevel
                })
                .ToListAsync();

            if (!logs.Any())
                return NotFound("No mood logs found.");

            return Ok(logs);
        }
        [HttpGet("recommendation/{employeeId}")]
        public async Task<IActionResult> GetRecommendation(string employeeId)
        {
            var logs = await _context.MoodLogs
                .Where(x => x.EmployeeId == employeeId)
                .ToListAsync();

            if (!logs.Any())
                return NotFound("No mood logs found.");

            var avgMood = logs.Average(x => x.MoodLevel);
            var avgStress = logs.Average(x => x.StressLevel);

            string recommendation;

            if (avgMood <= 2 && avgStress >= 4)
            {
                recommendation = "High stress detected. Consider taking a wellness break and speaking with HR.";
            }
            else if (avgMood <= 3 && avgStress >= 3)
            {
                recommendation = "Moderate stress detected. Maintain work-life balance and monitor your wellbeing.";
            }
            else
            {
                recommendation = "You appear to be doing well. Keep maintaining healthy habits.";
            }

            return Ok(new WellnessRecommendationDTO
            {
                EmployeeId = employeeId,
                Recommendation = recommendation
            });
        }
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeLogs(string employeeId)
        {
            var logs = await _context.MoodLogs
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }
    }
}