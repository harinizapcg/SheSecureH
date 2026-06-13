using Microsoft.AspNetCore.Mvc;
using SheSecure.Safety_WellnessService.DTOs;
using SheSecure.Safety_WellnessService.Interfaces;

namespace SheSecure.Safety_WellnessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SafeReachController : ControllerBase
    {
        private readonly ISafeReachService _service;

        public SafeReachController(
            ISafeReachService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult>
            Create(CreateSafeReachDTO dto)
        {
            await _service.CreateAsync(dto);

            return Ok(
                "Safe Reach check created successfully");
        }

        [HttpPut("acknowledge")]
        public async Task<IActionResult>
            Acknowledge(
                AcknowledgeSafeReachDTO dto)
        {
            await _service.AcknowledgeAsync(dto);

            return Ok(
                "Arrival acknowledged successfully");
        }

        [HttpGet("all")]
        public async Task<IActionResult>
            GetAll()
        {
            return Ok(
                await _service.GetAllAsync());
        }
        [HttpPut("escalate/{id}")]
        public async Task<IActionResult> Escalate(int id)
        {
            try
            {
                await _service.EscalateAsync(id);

                return Ok("Safe Reach check escalated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            return Ok(
                await _service.GetByIdAsync(id));
        }
    }
}